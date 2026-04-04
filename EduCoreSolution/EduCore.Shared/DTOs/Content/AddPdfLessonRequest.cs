namespace EduCore.Shared.DTOs.Content
{
    public class AddPdfLessonRequest
    {
        public string FileUrl { get; set; } = string.Empty;
        public int? FileSizeKb { get; set; }
    }
}
