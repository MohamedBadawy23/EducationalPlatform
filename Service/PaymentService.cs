using Core.Entities;
using Core.ServiceContract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RepositoryProject;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class PaymentService : IPaymentService
{
    private readonly EducationalPlatformDbContext _dbContext;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public PaymentService(EducationalPlatformDbContext dbContext, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _config = config;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<(Enrollment Enrollment, string RedirectUrl)> ProcessPaymentAsync(int enrollmentId, string paymentMethod, decimal amount)
    {
        var enrollment = await _dbContext.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Subject)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);

        if (enrollment == null) throw new KeyNotFoundException($"Enrollment {enrollmentId} not found.");
        if (enrollment.Student == null) throw new InvalidOperationException("Student not found.");

        int integrationId = GetIntegrationId(paymentMethod);
        if (integrationId == 0) throw new ArgumentException($"No integration ID configured for method '{paymentMethod}'");

        int amountCents = (int)(amount * 100);
        int specialReference = RandomNumberGenerator.GetInt32(1000000, 9999999) + enrollmentId;

        // Step 1: Generate auth_token
        var apiKey = _config["Paymob:APIKey"] ?? throw new ArgumentException("Paymob APIKey not configured");
        var authTokenPayload = new { api_key = apiKey };
        var authResponse = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", authTokenPayload);
        var authContent = await authResponse.Content.ReadAsStringAsync();
        if (!authResponse.IsSuccessStatusCode) throw new Exception($"Auth token request failed: {authContent}");

        var authJson = JsonDocument.Parse(authContent);
        var authToken = authJson.RootElement.GetProperty("token").GetString();

        // Step 2: Prepare payment key payload
        var billingData = new
        {
            apartment = "N/A",
            first_name = enrollment.Student.FullName ?? "Guest",
            last_name = "N/A",
            phone_number = enrollment.Student.PhoneNumber,
            country = "EG",
            email = enrollment.Student.Email,
            city = "N/A",
            state = "N/A",
            street = "N/A",
            building = "N/A",
            floor = "N/A"
        };

        var paymentKeyPayload = new
        {
            auth_token = authToken,
            amount_cents = amountCents,
            expiration = 3600,
            order_id = specialReference,
            billing_data = billingData,
            currency = "EGP",
            integration_id = integrationId,
            items = new[]
            {
                new
                {
                    name = $"Enrollment #{specialReference-145}",
                    amount_cents = amountCents,
                    quantity = 1,
                    description = $"Payment for course {enrollment.Subject.Name}"
                }
            }
        };

        var paymentKeyResponse = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", paymentKeyPayload);
        var paymentKeyContent = await paymentKeyResponse.Content.ReadAsStringAsync();
        if (!paymentKeyResponse.IsSuccessStatusCode) throw new Exception($"Payment key request failed: {paymentKeyContent}");

        var paymentKeyJson = JsonDocument.Parse(paymentKeyContent);
        var clientSecret = paymentKeyJson.RootElement.GetProperty("token").GetString();
        // Step 3: Save payment record
        var payment = new Payment
        {
            Amount = enrollment.Subject.Price,
            PaymentMethod = paymentMethod,
            Status = "Pending",
            TransactionId = specialReference.ToString(),
            EnrlollmentId = enrollment.Id,
            PaymentDate = DateTime.Now,
            UserId = enrollment.Student.Id.ToString(),
        };

        _dbContext.Payments.Add(payment);
        enrollment.PaymentStatus = "Pending";
        await _dbContext.SaveChangesAsync();

        // Step 4: Generate redirect URL
        string publicKey = _config["Paymob:PublicKey"];
        string redirectUrl = $"https://accept.paymob.com/api/acceptance/iframe?payment_token={clientSecret}&public_key={publicKey}";

        return (enrollment, redirectUrl);
    }

    private int GetIntegrationId(string paymentMethod) =>
        paymentMethod?.ToLower() switch
        {
            "card" => int.Parse(_config["Paymob:CardIntegrationId"] ?? "0"),
            "wallet" => int.Parse(_config["Paymob:MobileIntegrationId"] ?? "0"),
            _ => 0
        };

    public async Task<Enrollment> UpdateOrderSuccess(string specialReference)
    {
        var payment = await _dbContext.Payments
            .Include(p => p.Enrollment)
            .ThenInclude(e => e.Student)
            .Include(p => p.Enrollment)
            .ThenInclude(e => e.Subject)
            .FirstOrDefaultAsync(p => p.TransactionId == specialReference);

        if (payment == null) throw new KeyNotFoundException($"Payment {specialReference} not found.");

        payment.Status = "Success";
        payment.Enrollment.PaymentStatus = "Success";

        await _dbContext.SaveChangesAsync();
        return payment.Enrollment;
    }

    public async Task<Enrollment> UpdateOrderFailed(string specialReference)
    {
        var payment = await _dbContext.Payments
            .Include(p => p.Enrollment)
            .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(p => p.TransactionId == specialReference);

        if (payment == null) throw new KeyNotFoundException($"Payment {specialReference} not found.");

        payment.Status = "Failed";
        payment.Enrollment.PaymentStatus = "Failed";

        await _dbContext.SaveChangesAsync();
        return payment.Enrollment;
    }

    // HMAC SHA512 للتحقق من Webhook
    public string ComputeHmacSHA512(string data, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hash = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}