using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.AuthModel
{
    public class AuditLog : BaseEntity<int>
    {
        public int UserId { get; set; }
        public string Action { get; set; } = null!;   // e.g. "course.create"
        public string EntityType { get; set; } = null!;   // e.g. "Course"
        public int? EntityId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}
