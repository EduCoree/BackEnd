using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.EnrollmentDTOs;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/cash-requests")]
    [Authorize(Roles = "Admin")]
    public class AdminCashRequestController : ControllerBase
    {
        private readonly ICashPaymentRequestService _service;

        public AdminCashRequestController(ICashPaymentRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllRequestsAsync();
            return Ok(ApiResponse<IEnumerable<CashPaymentRequestDto>>.SuccessResult(result));
        }

        [HttpPut("{id:int}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _service.ConfirmRequestAsync(id);
            return Ok(ApiResponse<CashPaymentRequestDto>.SuccessResult(result, "Request confirmed successfully."));
        }

        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var result = await _service.RejectRequestAsync(id);
            return Ok(ApiResponse<CashPaymentRequestDto>.SuccessResult(result, "Request rejected."));
        }
    }
}
