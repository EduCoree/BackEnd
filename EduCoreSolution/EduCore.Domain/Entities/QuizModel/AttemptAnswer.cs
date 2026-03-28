using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.QuizModel
{
    // Supports MCQ and TrueFalse only — student picks an AnswerOption
    public class AttemptAnswer:BaseEntity<int>
    {
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerOptionId { get; set; }   // chosen option (MCQ or True/False)

        // Navigation
        public QuizAttempt Attempt { get; set; } = null!;
        public Question Question { get; set; } = null!;
        public AnswerOption AnswerOption { get; set; } = null!;
    }
}
