using EduCore.Domain.Entities.AuthModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.NotificationsModel
{
    
        public class Notification : BaseEntity<int>
    {
            public string UserId { get; set; }
            public string Type { get; set; } = null!;   // e.g. "quiz_result", "enrollment"
            public string Title { get; set; } = null!;
            public string Message { get; set; } = null!;
            public bool IsRead { get; set; } = false;
            public DateTime CreatedAt { get; set; }

            // Navigation
            public User User { get; set; } = null!;
        }
    
}
