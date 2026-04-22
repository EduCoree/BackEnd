using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.EnrollmentDTOs;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    [Authorize(Roles = "Student")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        private string StudentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost("free/{courseId:int}")]
        public async Task<IActionResult> EnrollFree(int courseId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(ApiResponse<object>.FailResult("You must log in first."));
            }

            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _enrollmentService.EnrollFreeAsync(StudentId, courseId);
            return Ok(ApiResponse<EnrollmentDto>.SuccessResult(result, "I enrolled in the course"));
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(ApiResponse<object>.FailResult("You must log in first."));
            }
            var result = await _enrollmentService.CreateCheckoutAsync(StudentId, dto.CourseId);
            return Ok(ApiResponse<CheckoutResponseDto>.SuccessResult(result, "Payment link has been created"));
        }


        [HttpPost("webhooks/paymob")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymobWebhook()  
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                Console.WriteLine($"[PAYMOB WEBHOOK] Body: {body}");
                if (string.IsNullOrWhiteSpace(body))
                {
                    return Ok("Empty body");  
                }

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var webhook = System.Text.Json.JsonSerializer.Deserialize<PaymobWebhookDto>(body, options);


                if (webhook?.obj == null)
                {
                    Console.WriteLine("[PAYMOB] webhook or obj is null");
                    return Ok("Invalid webhook");
                }
                Console.WriteLine($"[PAYMOB] top-level success={webhook.success}");
                Console.WriteLine($"[PAYMOB] obj.success={webhook.obj.success}");
                Console.WriteLine($"[PAYMOB] obj.pending={webhook.obj.pending}");
                Console.WriteLine($"[PAYMOB] obj.error_occured={webhook.obj.error_occured}");
                Console.WriteLine($"[PAYMOB] merchant_order_id={webhook.obj?.order?.merchant_order_id}");


                bool isPaymentSuccessful = webhook.obj.success == true &&
                                  webhook.obj.pending == false &&
                                  webhook.obj.error_occured != true;

                if (!isPaymentSuccessful)
                {
                    Console.WriteLine("[PAYMOB] Payment not successful - skipping");
                    return Ok("Payment failed");
                }

                await _enrollmentService.HandlePaymobWebhookAsync(webhook);
                Console.WriteLine("[PAYMOB] Enrollment completed ✅");
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok($"Error: {ex.Message}");  
            }
        }

        // Test GET
        [HttpGet("webhooks/paymob-test")]
        [AllowAnonymous]
        public IActionResult TestWebhookGet()
        {
            var result = new
            {
                status = "OK",
                message = "Paymob webhook endpoint is accessible",
                timestamp = DateTime.UtcNow,
                url = $"{Request.Scheme}://{Request.Host}{Request.Path}"
            };
            return Ok(result);
        }

        [HttpPost("complete-payment/{paymentId:int}")]
        public async Task<IActionResult> CompletePayment(int paymentId)
        {
            try
            {
                var webhook = new PaymobWebhookDto
                {
                    type = "TRANSACTION",
                    success = true,
                    hmac = "manual-test",
                    obj = new PaymobTransactionObj
                    {
                        id = 999999,
                        success = true,
                        amount_cents = 400,
                        currency = "EGP",
                        created_at = DateTime.UtcNow.ToString("o"),
                        order = new PaymobOrder
                        {
                            merchant_order_id = paymentId.ToString()
                        },
                        source_data = new PaymobSourceData
                        {
                            type = "card",
                            sub_type = "MasterCard",
                            pan = "1234"
                        }
                    }
                };

                await _enrollmentService.HandlePaymobWebhookAsync(webhook);
                return Ok(ApiResponse<object>.SuccessResult(null, "Payment completed"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResult($"Error: {ex.Message}"));
            }
        }

        [HttpGet("payment-redirect")]
        [AllowAnonymous]
        public IActionResult PaymentRedirect([FromQuery] bool success)
        {
            var frontendUrl = "http://localhost:4200";

            if (success)
                return Redirect($"{frontendUrl}/#/payment/success?fromCard=true");
            else
                return Redirect($"{frontendUrl}/#/payment/failed");
        }


    }
}
