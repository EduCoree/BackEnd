namespace EduCore.Shared.DTOs.Content
{
    public class UpdateLessonRequest
    {
        public string? Title { get; set; }
        public int? DurationSeconds { get; set; }
        public int? SortOrder { get; set; }
    }
}
