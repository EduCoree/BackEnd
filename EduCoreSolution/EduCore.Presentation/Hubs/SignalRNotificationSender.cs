using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Notifications;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Hubs
{
    public class SignalRNotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationSender(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendAsync(string userId, NotificationDto dto)
        {
            await _hubContext.Clients.User(userId).SendAsync("RecieveNotification", dto);
        }
    }
}
