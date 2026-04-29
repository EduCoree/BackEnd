using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.LessonAi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [Route("api/lesson-ai")]
    [Authorize(Roles = "Student")]
    public class LessonAiController(ILessonAiService lessonAiService) : ApiBaseController
    {
        [HttpPost("ask")]
        public async Task<ActionResult> Ask([FromBody] LessonAiRequestDto dto, CancellationToken ct)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await lessonAiService.AskAsync(studentId, dto, ct);
            return HandleResult(result);
        }

        [HttpPost("summarize")]
        public async Task<ActionResult> Summarize([FromBody] LessonAiRequestDto dto, CancellationToken ct)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await lessonAiService.SummarizeAsync(studentId, dto, ct);
            return HandleResult(result);
        }

        [HttpPost("translate")]
        public async Task<ActionResult> Translate([FromBody] LessonAiRequestDto dto, CancellationToken ct)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await lessonAiService.TranslateAsync(studentId, dto, ct);
            return HandleResult(result);
        }

        [HttpPost("transcribe")]
        public async Task<ActionResult> Transcribe([FromBody] LessonAiRequestDto dto, CancellationToken ct)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await lessonAiService.TranscribeAsync(studentId, dto.LessonId, ct);
            return HandleResult(result);
        }
    }
}
