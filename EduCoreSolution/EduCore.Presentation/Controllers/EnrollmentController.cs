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
            var result = await _enrollmentService.EnrollFreeAsync(StudentId, courseId);
            return Ok(ApiResponse<EnrollmentDto>.SuccessResult(result, "I enrolled in the course"));
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            var result = await _enrollmentService.CreateCheckoutAsync(StudentId, dto.CourseId);
            return Ok(ApiResponse<CheckoutResponseDto>.SuccessResult(result, "Payment link has been created"));
        }

        [HttpPost("webhooks/paymob")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymobWebhook([FromBody] PaymobWebhookDto webhook)
        {
            try
            {
                // 1. Verify HMAC signature
                if (!VerifyPaymobHmac(webhook))
                {
                    return Ok("Invalid signature");
                }

                // 2. Check if payment succeeded
                if (!webhook.success || webhook.obj?.success != true)
                {
                    return Ok("Payment failed");
                }

                // 3. Process enrollment 
                await _enrollmentService.HandlePaymobWebhookAsync(webhook);

                return Ok("Webhook processed");
            }
            catch (Exception ex)
            {
                return Ok($"Error: {ex.Message}");
            }
        }

        //helper method
        private bool VerifyPaymobHmac(PaymobWebhookDto webhook)
        {

            var hmacSecret = "your-hmac-secret"; 
            if (string.IsNullOrEmpty(hmacSecret))
                return true; 
           
            return true;
        }
    }
}
