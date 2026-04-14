using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [Route("api/chat")]
    [Authorize]
    public class ChatController(IChatService chatService) : ApiBaseController
    {
        [HttpPost]
        public async Task<ActionResult> SendMessage([FromBody] ChatRequestDto dto, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "Student";
            var result = await chatService.SendMessageAsync(userId, userRole, dto, ct);
            return HandleResult(result);
        }

        [HttpDelete("history")]
        public async Task<ActionResult> ClearHistory(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await chatService.ClearHistoryAsync(userId, ct);
            return HandleResult(result);
        }
    }
}
