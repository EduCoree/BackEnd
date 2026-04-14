using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Teacher
{
    public class QuizDetailsDto
    {
        public int Id { get; init; }
        public int CourseId { get; init; }
        public string Title { get; init; } = string.Empty;
        public int? TimeLimitMins { get; init; }
        public int PassScore { get; init; }
        public int? MaxAttempts { get; init; }
        public bool IsRandomized { get; init; }
        public ICollection<QuestionDto> Questions { get; init; } = new List<QuestionDto>();
    }

}
