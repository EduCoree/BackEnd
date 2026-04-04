namespace EduCore.Shared.DTOs.Content
{
    public class AddVideoLessonRequest
    {
        public string VideoUrl { get; set; } = string.Empty;
        public string VideoProvider { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
    }
}
