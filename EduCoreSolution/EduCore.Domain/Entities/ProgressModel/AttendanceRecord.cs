using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.ProgressModel
{
    public enum AttendanceStatus { Attended, Absent, Late }

    public class AttendanceRecord : BaseEntity<int>
    {
        public int StudentId { get; set; }
        public int LiveSessionId { get; set; }
        public DateTime JoinedAt { get; set; }
        public AttendanceStatus Status { get; set; }

        // Navigation
        public User Student { get; set; } = null!;
        public LiveSession LiveSession { get; set; } = null!;
    }
}
