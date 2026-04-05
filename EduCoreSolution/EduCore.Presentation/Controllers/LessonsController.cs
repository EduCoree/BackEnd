using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Content;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/teacher/courses/{courseId:int}/lessons")]
    [Authorize(Roles = "Teacher")]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _service;

        public LessonsController(ILessonService service)
        {
            _service = service;
        }

        private string TeacherId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost]
        public async Task<IActionResult> CreateLesson(
            int courseId, [FromBody] CreateLessonRequest request, CancellationToken ct)
        {
            var result = await _service.CreateLessonAsync(courseId, TeacherId, request, ct);
            return StatusCode(201, ApiResponse<LessonResponse>.SuccessResult(result, "Lesson created successfully."));
        }

        [HttpPut("{lessonId:int}")]
        public async Task<IActionResult> UpdateLesson(
            int courseId, int lessonId, [FromBody] UpdateLessonRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateLessonAsync(courseId, lessonId, TeacherId, request, ct);
            return Ok(ApiResponse<LessonResponse>.SuccessResult(result, "Lesson updated successfully."));
        }

        [HttpDelete("{lessonId:int}")]
        public async Task<IActionResult> DeleteLesson(
            int courseId, int lessonId, CancellationToken ct)
        {
            await _service.DeleteLessonAsync(courseId, lessonId, TeacherId, ct);
            return NoContent();
        }

        [HttpPost("{lessonId:int}/video")]
        public async Task<IActionResult> AddVideo(
            int courseId, int lessonId, [FromBody] AddVideoLessonRequest request, CancellationToken ct)
        {
            var result = await _service.AddVideoAsync(courseId, lessonId, TeacherId, request, ct);
            return StatusCode(201, ApiResponse<VideoLessonResponse>.SuccessResult(result, "Video added successfully."));
        }

        [HttpDelete("{lessonId:int}/video")]
        public async Task<IActionResult> RemoveVideo(
            int courseId, int lessonId, CancellationToken ct)
        {
            await _service.RemoveVideoAsync(courseId, lessonId, TeacherId, ct);
            return NoContent();
        }

        [HttpPost("{lessonId:int}/pdf")]
        public async Task<IActionResult> AddPdf(
            int courseId, int lessonId, [FromBody] AddPdfLessonRequest request, CancellationToken ct)
        {
            var result = await _service.AddPdfAsync(courseId, lessonId, TeacherId, request, ct);
            return StatusCode(201, ApiResponse<PdfLessonResponse>.SuccessResult(result, "PDF added successfully."));
        }

        [HttpDelete("{lessonId:int}/pdf")]
        public async Task<IActionResult> RemovePdf(
            int courseId, int lessonId, CancellationToken ct)
        {
            await _service.RemovePdfAsync(courseId, lessonId, TeacherId, ct);
            return NoContent();
        }

        [HttpPut("{lessonId:int}/free-preview")]
        public async Task<IActionResult> ToggleFreePreview(
            int courseId, int lessonId, [FromBody] ToggleFreePreviewRequest request, CancellationToken ct)
        {
            await _service.ToggleFreePreviewAsync(courseId, lessonId, TeacherId, request.IsFreePreview, ct);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Free preview toggled successfully."));
        }
    }
}
