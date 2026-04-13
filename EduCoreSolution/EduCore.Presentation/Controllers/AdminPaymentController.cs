using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.EnrollmentDTOs;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/payments")]
    [Authorize(Roles = "Admin")]
    public class AdminPaymentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IPaymentService _paymentService;

        public AdminPaymentController(
            IEnrollmentService enrollmentService,
            IPaymentService paymentService)
        {
            _enrollmentService = enrollmentService;
            _paymentService = paymentService;
        }

        [HttpPost("cash")]
        public async Task<IActionResult> RecordCashPayment([FromBody] CashPaymentDto dto)
        {
            var result = await _enrollmentService.RecordCashPaymentAsync(dto);
            return Ok(ApiResponse<EnrollmentDto>.SuccessResult(result, "تم تسجيل الدفع النقدي"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] PaginationParams pagination)
        {
            var payments = await _paymentService.GetAllPaymentsAsync(pagination);
            return Ok(ApiResponse<PagedResult<PaymentDto>>.SuccessResult(payments));
        }
    }
}
