using System;

namespace EduCore.Shared.DTOs.Progress
{
    public class LessonProgressResponse
    {
        public int LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public int LastPositionSecs { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
