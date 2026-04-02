using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.CourseDTOs;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/teacher/courses")]
    [Authorize(Roles = "Teacher")]
    public class TeacherCoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IImageService _imageService;

        public TeacherCoursesController(ICourseService courseService, IImageService imageService)
        {
            _courseService = courseService;
            _imageService = imageService;
        }

        // بنجيب الـ ID من الـ Identity Cookie
        private string TeacherId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetMyCourses([FromQuery] PaginationParams pagination)
        {
            var result = await _courseService.GetTeacherCoursesAsync(TeacherId, pagination);
            return Ok(ApiResponse<PagedResult<CourseSummaryDto>>.SuccessResult(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            var course = await _courseService.CreateCourseAsync(TeacherId, dto);
            return CreatedAtAction(nameof(GetById), new { id = course.Id },
                ApiResponse<CourseSummaryDto>.SuccessResult(course, "The course has been created"));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _courseService.GetTeacherCourseByIdAsync(id, TeacherId);
            return Ok(ApiResponse<CourseDetailDto>.SuccessResult(course));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto dto)
        {
            var result = await _courseService.UpdateCourseAsync(id, TeacherId, dto);
            return Ok(ApiResponse<CourseSummaryDto>.SuccessResult(result, "Updated"));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.DeleteCourseAsync(id, TeacherId);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Deleted"));
        }

        [HttpPut("{id:int}/cover")]
        public async Task<IActionResult> UploadCover(int id, IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest(ApiResponse<string>.FailResult("Please select an image"));

            var imageUrl = await _imageService.UploadImageAsync(file, "educore/courses");
            await _courseService.UpdateCoverImageAsync(id, TeacherId, imageUrl);
            return Ok(ApiResponse<string>.SuccessResult(imageUrl, "The image has been uploaded"));
        }

        [HttpPut("{id:int}/pricing")]
        public async Task<IActionResult> UpdatePricing(int id, [FromBody] UpdatePricingDto dto)
        {
            await _courseService.UpdatePricingAsync(id, TeacherId, dto);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Price has been updated"));
        }

        [HttpPut("{id:int}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            await _courseService.PublishCourseAsync(id, TeacherId);
            return Ok(ApiResponse<bool>.SuccessResult(true, "The course has been published"));
        }

        [HttpPut("{id:int}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            await _courseService.UnpublishCourseAsync(id, TeacherId);
            return Ok(ApiResponse<bool>.SuccessResult(true, "The course has been stopped"));
        }

        // ── Sections ──────────────────────────────────────────────

        [HttpGet("{id:int}/sections")]
        public async Task<IActionResult> GetSections(int id)
        {
            var sections = await _courseService.GetCourseSectionsAsync(id);
            return Ok(ApiResponse<List<SectionDto>>.SuccessResult(sections));
        }

        [HttpPost("{id:int}/sections")]
        public async Task<IActionResult> AddSection(int id, [FromBody] CreateSectionDto dto)
        {
            var section = await _courseService.AddSectionAsync(id, TeacherId, dto);
            return Ok(ApiResponse<SectionDto>.SuccessResult(section, "Added"));
        }

        [HttpPut("{id:int}/sections/{sectionId:int}")]
        public async Task<IActionResult> UpdateSection(
            int id, int sectionId, [FromBody] UpdateSectionDto dto)
        {
            await _courseService.UpdateSectionAsync(id, sectionId, TeacherId, dto);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Updated"));
        }

        [HttpDelete("{id:int}/sections/{sectionId:int}")]
        public async Task<IActionResult> DeleteSection(int id, int sectionId)
        {
            await _courseService.DeleteSectionAsync(id, sectionId, TeacherId);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Deleted"));
        }

        [HttpPut("{id:int}/sections/reorder")]
        public async Task<IActionResult> ReorderSections(
            int id, [FromBody] List<ReorderItemDto> items)
        {
            await _courseService.ReorderSectionsAsync(id, TeacherId, items);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Reordered"));
        }

        [HttpPut("{id:int}/sections/{sectionId:int}/lessons/reorder")]
        public async Task<IActionResult> ReorderLessons(
            int id, int sectionId, [FromBody] List<ReorderItemDto> items)
        {
            await _courseService.ReorderLessonsAsync(id, sectionId, TeacherId, items);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Reordered"));
        }
    }
}