using Core.Entities;
using Core.ServiceContract;
using Core.Specefications;
using CoreProject.Enteties;
using CoreProject.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Paymob_Integration_DotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<Enrollment> _enrollmentRepository;
        private readonly IGenericRepository<Subject> _subjectRepository;
        private readonly IGenericRepository<Student> _studentRepository;

        public StudentController(
            IPaymentService paymentService,
            IConfiguration configuration,
            IGenericRepository<Enrollment> enrollmentRepository,
            IGenericRepository<Subject> subjectRepository,
            IGenericRepository<Student> studentRepository)
        {
            _paymentService = paymentService;
            _configuration = configuration;
            _enrollmentRepository = enrollmentRepository;
            _subjectRepository = subjectRepository;
            _studentRepository = studentRepository;
        }

        /// <summary>
        /// Creates a payment token for a subject enrollment.
        /// </summary>
        [HttpPost("create-payment-token")]
        public async Task<IActionResult> CreatePaymentToken(
           [FromQuery] int subjectId,
           [FromQuery] string paymentMethod)
        {
            if (subjectId <= 0)
                return BadRequest(new { Message = "Invalid subject ID." });

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "User not authenticated." });

            var studentSpec = new StudentByIdSpec(userId);
            var student = await _studentRepository.GetByIdWithSpecAsync(studentSpec);
            if (student == null)
                return NotFound(new { Message = "Student not found in DB." });

            var subjectSpec = new SubjectByIdSpec(subjectId);
            var subject = await _subjectRepository.GetByIdWithSpecAsync(subjectSpec);
            if (subject == null)
                return NotFound(new { Message = "Subject not found." });

            if (string.IsNullOrWhiteSpace(paymentMethod))
                return BadRequest(new { Message = "Payment method is required." });

            if (!paymentMethod.Equals("card", StringComparison.OrdinalIgnoreCase) &&
                !paymentMethod.Equals("wallet", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { Message = "Invalid payment method. Supported methods: 'card', 'wallet'." });
            }

            // عمل Enrollment جديد
            var enrollment = new Enrollment
            {
                SubjectId = subject.Id,
                StudentId = student.Id,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Active",
                PaymentStatus = "Pending",
                Progress = 0
            };
            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangeAsync();

            try
            {
                decimal totalAmount = subject.Price;
                var (enrollmentResult, redirectUrl) = await _paymentService
                    .ProcessPaymentAsync(enrollment.Id, paymentMethod, totalAmount);

                return Ok(new
                {
                    EnrollmentId = enrollment.Id,
                    RedirectUrl = redirectUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error processing payment: {ex.Message}" });
            }
        }
        /// <summary>
        /// Handles Paymob redirect after payment (frontend display only).
        /// </summary>
        [HttpGet("callback")]
        public IActionResult Callback()
        {
            var query = Request.Query;
            string receivedHmac = query["hmac"];
            string calculatedHmac = _paymentService.ComputeHmacSHA512(
                string.Join("", query.Select(q => q.Value)),
                _configuration["Paymob:HMAC"]
            );
            if (!receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { Message = "Invalid HMAC." });

            bool.TryParse(query["success"], out bool isSuccess);
            var merchantOrderId = query["merchant_order_id"].ToString();

            return Ok(new
            {
                Success = isSuccess,
                MerchantOrderId = merchantOrderId
            });
        }

        /// <summary>
        /// Handles Paymob server-to-server callback.
        /// </summary>
        [HttpPost("server-callback")]
        [AllowAnonymous] // Paymob webhook does not send JWT
        public async Task<IActionResult> ServerCallback([FromBody] JsonElement payload)
        {
            try
            {
                string secret = _configuration["Paymob:HMAC"];
                string receivedHmac = Request.Headers["x-hmac-signature"];

                string payloadString = payload.GetRawText();
                string calculatedHmac = _paymentService.ComputeHmacSHA512(payloadString, secret);

                if (!receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
                    return Unauthorized(new { Message = "Invalid HMAC" });

                if (payload.TryGetProperty("order", out var order))
                {
                    string merchantOrderId = order.GetProperty("merchant_order_id").GetString();
                    bool isSuccess = order.GetProperty("success").GetBoolean();

                    if (!string.IsNullOrEmpty(merchantOrderId))
                    {
                        if (isSuccess)
                            await _paymentService.UpdateOrderSuccess(merchantOrderId);
                        else
                            await _paymentService.UpdateOrderFailed(merchantOrderId);
                    }
                }

                return Ok(new { Message = "Callback processed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error processing server callback: {ex.Message}" });
            }
        }
    }
}