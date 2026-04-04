using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Content;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/teacher/courses/{courseId:int}/sessions")]
    [Authorize(Roles = "Teacher")]
    public class LiveSessionsController : ControllerBase
    {
        private readonly ILiveSessionService _service;

        public LiveSessionsController(ILiveSessionService service)
        {
            _service = service;
        }

        private string TeacherId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetSessions(
            int courseId, CancellationToken ct)
        {
            var result = await _service.GetSessionsAsync(courseId, TeacherId, ct);
            return Ok(ApiResponse<List<LiveSessionResponse>>.SuccessResult(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession(
            int courseId, [FromBody] CreateLiveSessionRequest request, CancellationToken ct)
        {
            var result = await _service.CreateSessionAsync(courseId, TeacherId, request, ct);
            return StatusCode(201, ApiResponse<LiveSessionResponse>.SuccessResult(result, "Session scheduled."));
        }

        [HttpPut("{sessionId:int}")]
        public async Task<IActionResult> UpdateSession(
            int courseId, int sessionId, [FromBody] UpdateLiveSessionRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateSessionAsync(courseId, sessionId, TeacherId, request, ct);
            return Ok(ApiResponse<LiveSessionResponse>.SuccessResult(result, "Session updated."));
        }

        [HttpDelete("{sessionId:int}")]
        public async Task<IActionResult> CancelSession(
            int courseId, int sessionId, CancellationToken ct)
        {
            await _service.DeleteSessionAsync(courseId, sessionId, TeacherId, ct);
            return NoContent();
        }

        [HttpPut("{sessionId:int}/recording")]
        public async Task<IActionResult> UpdateRecording(
            int courseId, int sessionId, [FromBody] UpdateRecordingRequest request, CancellationToken ct)
        {
            await _service.UpdateRecordingAsync(courseId, sessionId, TeacherId, request.RecordingUrl, ct);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Recording updated."));
        }
    }
}
