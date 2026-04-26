using EduCore.Domain.Entities.AuthModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.EnrollmentDTOs;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    [Authorize(Roles = "Student")]
    public class CashRequestController : ControllerBase
    {
        private readonly ICashPaymentRequestService _service;
        private string StudentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public CashRequestController(ICashPaymentRequestService service)
        {
            _service = service;
        }

        [HttpPost("request-cash/{courseId:int}")]
        public async Task<IActionResult> RequestCash(int courseId)
        {
            var result = await _service.CreateRequestAsync(StudentId, courseId);
            return Ok(ApiResponse<CashPaymentRequestDto>.SuccessResult(result, "Cash request submitted successfully."));
        }
    }
}
