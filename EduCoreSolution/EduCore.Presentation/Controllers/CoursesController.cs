using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.CourseDTOs;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // GET /api/courses?categoryId=1&level=0&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll( [FromQuery] CourseFilterDto filter,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _courseService.GetAllPublishedAsync(filter, pagination);
            return Ok(ApiResponse<PagedResult<CourseSummaryDto>>.SuccessResult(result));
        }

        // GET /api/courses/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return Ok(ApiResponse<CourseDetailDto>.SuccessResult(course));
        }

        // GET /api/courses/my-courses
        [HttpGet("my-courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyCourses()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var courses = await _courseService.GetMyCoursesAsync(studentId);
            return Ok(ApiResponse<IEnumerable<StudentEnrolledCourseDto>>.SuccessResult(courses));
        }
    }
}