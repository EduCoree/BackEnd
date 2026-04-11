using EduCore.Shared.Common;
using EduCore.Shared.DTOs.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationListDto>> GetUserNotificationsAync(string useriD, PaginationParams pagination);
        Task<int> GetUnreadCountAsync(string UserId);
        Task MarkAllAsRead(string UserId);
        Task MarkAsRead(int NotificationId);

    }
}
