using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.ProgressModel;
using System;
using System.Collections.Generic;

namespace EduCore.Domain.Entities.ContentModel
{
    public enum LiveProvider { Zoom, MicrosoftTeams, GoogleMeet, Jitsi }

    public class LiveSession : BaseEntity<int>
    {
        public int CourseId { get; set; }
        public int? LessonId { get; set; }
        public LiveProvider Provider { get; set; }
        public string MeetingUrl { get; set; } = null!;
        public DateTime ScheduledAt { get; set; }
        public string? RecordingUrl { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Course Course { get; set; } = null!;
        public Lesson? Lesson { get; set; }
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
