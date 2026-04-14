using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.EnrollmentDTOs;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize(Roles = "Student")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private string StudentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyPayments()
        {
            var payments = await _paymentService.GetMyPaymentsAsync(StudentId);
            return Ok(ApiResponse<IEnumerable<PaymentDto>>.SuccessResult(payments));
        }
    }
}
