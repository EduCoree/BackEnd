using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.ProgressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.ContentModel
{
    public enum LiveProvider { Zoom, MicrosoftTeams, GoogleMeet, Jitsi }

    public class LiveSession:BaseEntity<int>
    {
        public int LessonId { get; set; }
        public LiveProvider Provider { get; set; }
        public string MeetingUrl { get; set; } = null!;
        public DateTime ScheduledAt { get; set; }
        public string? RecordingUrl { get; set; }

        // Navigation
        public Lesson Lesson { get; set; } = null!;
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
