using System;
using System.Collections.Generic;

namespace EduCore.Shared.DTOs.Progress
{
    public class StudentLessonDetailResponse
    {
        public string StudentId { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public List<LessonDetailItem> Lessons { get; set; } = new();
    }

    public class LessonDetailItem
    {
        public int LessonId { get; set; }
        public string Title { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int LastPositionSecs { get; set; }
    }
}
