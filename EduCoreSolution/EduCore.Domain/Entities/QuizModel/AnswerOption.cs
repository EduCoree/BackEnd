using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.QuizModel
{
    public class AnswerOption : BaseEntity<int>
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCorrect { get; set; } = false;

        // Navigation
        public Question Question { get; set; } = null!;
        public ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
    }
}
