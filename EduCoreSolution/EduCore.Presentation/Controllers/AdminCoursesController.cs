using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.CourseDTOs;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/courses")]
    [Authorize(Roles = "Admin")]
    public class AdminCoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public AdminCoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] CourseFilterDto filter,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _courseService.GetAllCoursesAdminAsync(filter, pagination);
            return Ok(ApiResponse<PagedResult<CourseSummaryDto>>.SuccessResult(result));
        }

        [HttpPut("{id:int}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            await _courseService.AdminPublishAsync(id);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Published"));
        }

        [HttpPut("{id:int}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            await _courseService.AdminUnpublishAsync(id);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Stop Publish"));
        }

        [HttpPut("{id:int}/pricing")]
        public async Task<IActionResult> UpdatePricing(int id, [FromBody] UpdatePricingDto dto)
        {
            await _courseService.AdminUpdatePricingAsync(id, dto);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Price has been updated"));
        }
    }
}
