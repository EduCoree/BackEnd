using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.QuizModel
{
   

    public class Question : BaseEntity<int>
    {
        public int QuizId { get; set; }
        public string Text { get; set; } = null!;
        public QuestionType Type { get; set; }
        public int Points { get; set; } = 1;

        // Navigation
        public Quiz Quiz { get; set; } = null!;
        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
        public ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
    }
}
