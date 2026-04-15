using System;

namespace EduCore.Shared.DTOs.Content
{
    public class LiveSessionResponse
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string MeetingUrl { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string? RecordingUrl { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Enriched fields for student agenda grouping
        public string? CourseName { get; set; }
        public string? TeacherName { get; set; }
    }
}
