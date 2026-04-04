using System;

namespace EduCore.Shared.DTOs.Content
{
    public class UpdateLiveSessionRequest
    {
        public string? Provider { get; set; }
        public string? MeetingUrl { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
