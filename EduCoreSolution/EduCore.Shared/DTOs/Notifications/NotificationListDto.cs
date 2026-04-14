using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Notifications
{
    public class NotificationListDto
    {
         public IEnumerable<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
         public int UnreadCount { get; set; }
         public int TotalCount { get; set; }
    }
}
