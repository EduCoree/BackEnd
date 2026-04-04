namespace EduCore.Shared.DTOs.Content
{
    public class VideoLessonResponse
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string VideoProvider { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
    }
}
