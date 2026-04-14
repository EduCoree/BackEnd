namespace EduCore.Shared.DTOs.Progress
{
    public class CourseProgressResponse
    {
        public int CourseId { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public double PercentComplete { get; set; }
        public bool CertificateIssued { get; set; }
    }
}
