namespace EduCore.Shared.DTOs.Progress
{
    public class StudentProgressSummaryResponse
    {
        public string StudentId { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public double PercentComplete { get; set; }
    }
}
