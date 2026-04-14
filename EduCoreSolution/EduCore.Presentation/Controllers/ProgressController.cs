using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Progress;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/progress")]
    [Authorize(Roles = "Student")]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _service;

        public ProgressController(IProgressService service)
        {
            _service = service;
        }

        private string StudentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost("lessons/{lessonId:int}/watch")]
        public async Task<IActionResult> RecordWatch(
            int lessonId, [FromBody] WatchHeartbeatRequest request, CancellationToken ct)
        {
            await _service.RecordWatchAsync(StudentId, lessonId, request.PositionSecs, ct);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Progress recorded."));
        }

        [HttpPut("lessons/{lessonId:int}/complete")]
        public async Task<IActionResult> CompleteLesson(
            int lessonId, CancellationToken ct)
        {
            var result = await _service.CompleteLessonAsync(StudentId, lessonId, ct);
            return Ok(ApiResponse<LessonProgressResponse>.SuccessResult(result, "Lesson completed."));
        }

        [HttpGet("courses/{courseId:int}")]
        public async Task<IActionResult> GetCourseProgress(
            int courseId, CancellationToken ct)
        {
            var result = await _service.GetCourseProgressAsync(StudentId, courseId, ct);
            return Ok(ApiResponse<CourseProgressResponse>.SuccessResult(result));
        }

        [HttpGet("courses/{courseId:int}/resume")]
        public async Task<IActionResult> GetResumeLesson(
            int courseId, CancellationToken ct)
        {
            var result = await _service.GetResumeLessonAsync(StudentId, courseId, ct);
            return Ok(ApiResponse<ResumeLessonResponse>.SuccessResult(result));
        }
    }
}
