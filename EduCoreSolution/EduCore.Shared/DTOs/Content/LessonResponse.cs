using System;

namespace EduCore.Shared.DTOs.Content
{
    public class LessonResponse
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public int? DurationSeconds { get; set; }
        public bool IsFreePreview { get; set; }
        public DateTime CreatedAt { get; set; }
        public VideoLessonResponse? VideoLesson { get; set; }
        public PdfLessonResponse? PdfLesson { get; set; }
    }
}
