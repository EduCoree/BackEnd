using EduCore.Shared.Enums;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class StudentQuestionDto
    {
        public int Id { get; init; }
        public string Text { get; init; } = string.Empty;
        public QuestionType Type { get; init; }
        public int Points { get; init; }
        public ICollection<StudentAnswerOptionDto> AnswerOptions { get; init; } = new List<StudentAnswerOptionDto>();
    }
}