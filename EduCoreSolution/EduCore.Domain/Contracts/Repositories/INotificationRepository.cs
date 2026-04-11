using EduCore.Domain.Entities.NotificationsModel;
using EduCore.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface INotificationRepository:IGenericRepository<Notification,int>
    {
        Task <IEnumerable<Notification>> GetUserNotificationsAsync(string UserId, PaginationParams paginationParams);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAllAsReadAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task<int> GetTotalCountAsync(string userId);
    }
}
