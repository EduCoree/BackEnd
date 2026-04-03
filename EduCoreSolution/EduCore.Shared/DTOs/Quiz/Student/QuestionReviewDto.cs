using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class QuestionReviewDto
    {
        public int QuestionId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public string SelectedAnswerText { get; init; } = string.Empty;
        public string CorrectAnswerText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
        public int Points { get; init; }
    }
}
