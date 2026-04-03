using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public record StudentQuizDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public int? TimeLimitMins { get; init; }
        public int PassScore { get; init; }
        public int AttemptsLeft { get; init; }   
        public ICollection<StudentQuestionDto> Questions { get; init; } = new List<StudentQuestionDto>();
    }
}
