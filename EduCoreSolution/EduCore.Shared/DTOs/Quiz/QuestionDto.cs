using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz
{
    public class QuestionDto
    {
        public int Id { get; init; }
        public string Text { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public int Points { get; init; }
        public ICollection<AnswerOptionDto> AnswerOptions { get; init; } = new List<AnswerOptionDto>();
    }
}
