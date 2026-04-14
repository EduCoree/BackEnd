using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.Notifications;
using EduCore.Shared.DTOs.Quiz.Teacher;
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
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController: ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        protected string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] PaginationParams pagination)
        {
            var result = await _notificationService
                .GetUserNotificationsAync(UserId, pagination);
            return Ok(ApiResponse<NotificationListDto>.SuccessResult(result, "Notifications retrieved successfully."));
        }
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _notificationService.GetUnreadCountAsync(UserId);
            return Ok(ApiResponse<int>.SuccessResult(count));
        }
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsRead(UserId);
            return NoContent();
        }
        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            await _notificationService.MarkAsRead(notificationId);
            return NoContent();
        }
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> Delete(int notificationId)
        {
            await _notificationService.DeleteAsync(notificationId);
            return NoContent();
        }
    }
}
