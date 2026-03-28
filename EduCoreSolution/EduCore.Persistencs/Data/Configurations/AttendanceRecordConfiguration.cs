using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public enum AttendanceStatus { Attended, Absent, Late }

    public class AttendanceRecord
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public long LiveSessionId { get; set; }
        public DateTime JoinedAt { get; set; }
        public AttendanceStatus Status { get; set; }

        // Navigation
        public User Student { get; set; } = null!;
        public LiveSession LiveSession { get; set; } = null!;
    }
}
