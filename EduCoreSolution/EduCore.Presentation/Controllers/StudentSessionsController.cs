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
    [Route("api/sessions")]
    [Authorize(Roles = "Student")]
    public class StudentSessionsController : ControllerBase
    {
        private readonly ILiveSessionService _service;

        public StudentSessionsController(ILiveSessionService service)
        {
            _service = service;
        }

        private string StudentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingSessions(CancellationToken ct)
        {
            var result = await _service.GetUpcomingSessionsAsync(StudentId, ct);
            return Ok(ApiResponse<List<LiveSessionResponse>>.SuccessResult(result));
        }

        [HttpGet("{sessionId:int}/join")]
        public async Task<IActionResult> JoinSession(
            int sessionId, CancellationToken ct)
        {
            var joinUrl = await _service.GetJoinUrlAsync(sessionId, StudentId, ct);
            return Ok(ApiResponse<string>.SuccessResult(joinUrl, "Join URL retrieved successfully."));
        }
    }
}
