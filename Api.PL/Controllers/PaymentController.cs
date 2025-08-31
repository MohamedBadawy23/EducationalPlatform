    //using Core.Entities;
    //using Core.ServiceContract;
    //using CoreProject.Enteties;
    //using CoreProject.Repository;
    //using Microsoft.AspNetCore.Authorization;
    //using Microsoft.AspNetCore.Mvc;
    //using System.Security.Claims;
    //using System.Text.Json;

    //namespace Paymob_Integration_DotNet.Controllers
    //{
    //    [ApiController]
    //    [Route("api/[controller]")]
    //    public class PaymentController : ControllerBase
    //    {
    //        private readonly IPaymentService _paymentService;
    //        private readonly IConfiguration _configuration;
    //        private readonly IGenericRepository<Enrollment> _enrollmentRepository;
    //        private readonly IGenericRepository<Subject> _subjectRepository;

    //        public PaymentController(
    //            IPaymentService paymentService,
    //            IConfiguration configuration,
    //            IGenericRepository<Enrollment> enrollmentRepository,
    //            IGenericRepository<Subject> subjectRepository)
    //        {
    //            _paymentService = paymentService;
    //            _configuration = configuration;
    //            _enrollmentRepository = enrollmentRepository;
    //            _subjectRepository = subjectRepository;
    //        }

    //        /// <summary>
    //        /// Creates a payment token for a subject enrollment.
    //        /// </summary>
    //        [Authorize]
    //        [HttpPost("create-payment-token")]
    //        public async Task<IActionResult> CreatePaymentToken(
    //           [FromQuery] int subjectId,
    //           [FromQuery] string paymentMethod)
    //        {
    //            if (subjectId <= 0)
    //                return BadRequest(new { Message = "Invalid subject ID." });

    //            var studentId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //            if (string.IsNullOrEmpty(studentId))
    //                return Unauthorized(new { Message = "User not authenticated." });

    //            // التحقق من المادة
    //            var subject = await _subjectRepository.GetByIdAsync(subjectId);
    //            if (subject == null)
    //                return NotFound(new { Message = "Subject not found." });

    //            // عمل Enrollment جديد
    //            var enrollment = new Enrollment
    //            {
    //                SubjectId = subject.Id,
    //                StudentId = int.Parse(studentId),
    //                EnrollmentDate = DateTime.UtcNow,
    //                Status= "Active",
    //                PaymentStatus = "Pending",
    //                Progress = 0
    //            };
    //            await _enrollmentRepository.AddAsync(enrollment);

    //            try
    //            {
    //                decimal totalAmount = subject.Price;

    //                if (string.IsNullOrWhiteSpace(paymentMethod))
    //                    return BadRequest(new { Message = "Payment method is required." });

    //                if (paymentMethod.Equals("card", StringComparison.OrdinalIgnoreCase) ||
    //                    paymentMethod.Equals("wallet", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var (enrollmentResult, redirectUrl) = await _paymentService
    //                        .ProcessPaymentAsync(enrollment.Id, paymentMethod, totalAmount);

    //                    return Ok(new
    //                    {
    //                        EnrollmentId = enrollment.Id,
    //                        RedirectUrl = redirectUrl
    //                    });
    //                }

    //                return BadRequest(new { Message = "Invalid payment method. Supported methods: 'card', 'wallet'." });
    //            }
    //            catch (Exception ex)
    //            {
    //                return StatusCode(500, new { Message = $"Error processing payment: {ex.Message}" });
    //            }
    //        }

    //        /// <summary>
    //        /// Handles Paymob redirect after payment (for frontend if needed).
    //        /// Just returns JSON instead of HTML.
    //        /// </summary>
    //        [HttpGet("callback")]
    //        public IActionResult Callback()
    //        {
    //            var query = Request.Query;
    //            string receivedHmac = query["hmac"];
    //            string calculatedHmac = _paymentService.ComputeHmacSHA512(
    //                string.Join("", query.Select(q => q.Value)),
    //                _configuration["Paymob:HMAC"]
    //            );
    //            if (!receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
    //                return Unauthorized(new { Message = "Invalid HMAC." });

    //            bool.TryParse(query["success"], out bool isSuccess);
    //            var merchantOrderId = query["merchant_order_id"].ToString();

    //            return Ok(new
    //            {
    //                Success = isSuccess,
    //                MerchantOrderId = merchantOrderId
    //            });
    //        }

    //        /// <summary>
    //        /// Handles Paymob server-to-server callback.
    //        /// </summary>
    //        [HttpPost("server-callback")]
    //        public async Task<IActionResult> ServerCallback([FromBody] JsonElement payload)
    //        {
    //            try
    //            {
    //                string receivedHmac = Request.Query["hmac"];
    //                string secret = _configuration["Paymob:HMAC"];

    //                if (!payload.TryGetProperty("obj", out var obj))
    //                    return BadRequest(new { Message = "Missing 'obj' in payload." });

    //                string calculatedHmac = _paymentService.ComputeHmacSHA512(obj.ToString(), secret);

    //                if (!receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
    //                    return Unauthorized(new { Message = "Invalid HMAC" });

    //                string merchantOrderId = obj.GetProperty("order").GetProperty("merchant_order_id").GetString();
    //                bool isSuccess = obj.GetProperty("success").GetBoolean();

    //                if (!string.IsNullOrEmpty(merchantOrderId))
    //                {
    //                    if (isSuccess)
    //                        await _paymentService.UpdateOrderSuccess(merchantOrderId);
    //                    else
    //                        await _paymentService.UpdateOrderFailed(merchantOrderId);
    //                }

    //                return Ok(new { Message = "Callback processed successfully." });
    //            }
    //            catch (Exception ex)
    //            {
    //                return StatusCode(500, new { Message = $"Error processing server callback: {ex.Message}" });
    //            }
    //        }
    //    }
    //}