namespace EduCore.Shared.DTOs.Progress
{
    public class ResumeLessonResponse
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = null!;
        public int LastPositionSecs { get; set; }
    }
}
