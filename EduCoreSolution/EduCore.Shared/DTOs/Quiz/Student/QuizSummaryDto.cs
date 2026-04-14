using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class QuizSummaryDto
    {
        public int Id { get; init; }
        public int courseId { get; init; }
        public string Title { get; init; } = string.Empty;
        public int? TimeLimitMins { get; init; }
        public int TotalPoints { get; init; }
        public int QuestionCount { get; init; } 
        public int PassScore { get; init; }     
        public int MaxAttempts { get; init; }  
        public int AttemptsLeft { get; init; }
    }
}
