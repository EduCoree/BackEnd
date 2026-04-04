namespace EduCore.Shared.DTOs.Content
{
    public class PdfLessonResponse
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public int? FileSizeKb { get; set; }
    }
}
