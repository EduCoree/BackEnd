using EduCore.Shared.DTOs.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface INotificationSender
    {
        Task SendAsync(string userId, NotificationDto dto);
    }
}
