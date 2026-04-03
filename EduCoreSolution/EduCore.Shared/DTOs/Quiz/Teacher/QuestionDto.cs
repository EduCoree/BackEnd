using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Teacher
{
    public class QuestionDto
    {
        public int Id { get; init; }
        public string Text { get; init; } = string.Empty;
        public QuestionType Type { get; init; }
        public int Points { get; init; }
        public ICollection<AnswerOptionDto> AnswerOptions { get; init; } = new List<AnswerOptionDto>();
    }
}
