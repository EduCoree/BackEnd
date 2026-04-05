namespace EduCore.Shared.DTOs.Content
{
    public class CreateLessonRequest
    {
        public int SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? DurationSeconds { get; set; }
        public int? SortOrder { get; set; }
    }
}
