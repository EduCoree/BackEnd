namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class AnswerOptionDto
    {
        public int Id { get; init; }
        public string Text { get; init; } = string.Empty;
        public bool IsCorrect { get; init; } 
    }
}