using EduCore.Shared.Common;
using EduCore.Shared.DTOs.Notifications;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface INotificationService
    {
        Task<NotificationListDto> GetUserNotificationsAync(string userId, PaginationParams pagination);
        Task<int> GetUnreadCountAsync(string UserId);
        Task MarkAllAsRead(string UserId);
        Task MarkAsRead(int NotificationId);
        Task DeleteAsync(int notificationId);

        Task SendNotificationAsync(string userId, string title, string message, NotificationType notificationType,int entityId, object? extraData = null);
        Task SendNotificationToAdminsAsync(string title, string message, NotificationType notificationType, int entityId);

    }
}
