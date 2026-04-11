using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.NotificationsModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    internal class NotificationRepository : GenericRepository<Notification, int>, INotificationRepository
    {
        public NotificationRepository(EduCoreDbContext context): base(context) { }
        public async Task<int> GetTotalCountAsync(string userId)
        {
          return await _EduCoreDbContext.Notifications
                .CountAsync(n=>n.UserId == userId);
            
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _EduCoreDbContext.Notifications
                .CountAsync(n=>n.UserId == userId && n.IsRead==false);
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string UserId, PaginationParams pagination)
        {
            return await _EduCoreDbContext.Notifications
                .Where(n => n.UserId == UserId).
                OrderByDescending(n => n.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize).ToListAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        { // i dont load the notification in variable and make the edit in the database for better performance
            await _EduCoreDbContext.Notifications
         .Where(n => n.UserId == userId && !n.IsRead)
         .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _EduCoreDbContext.Notifications
         .Where(n => n.Id == notificationId)
         .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

        }
    }
}
