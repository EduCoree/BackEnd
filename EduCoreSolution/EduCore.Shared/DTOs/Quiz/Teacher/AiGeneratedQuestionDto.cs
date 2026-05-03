using EduCore.Shared.Enums;

namespace EduCore.Shared.DTOs.Quiz.Teacher
{
    public class AiGeneratedQuestionDto
    {
        public string Text { get; init; } = string.Empty;
        public string Type { get; init; } = "MCQ";
        public int Points { get; init; }
        public List<AiGeneratedOptionDto> Options { get; init; } = new();
    }
}