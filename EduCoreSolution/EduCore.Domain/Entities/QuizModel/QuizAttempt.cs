using EduCore.Domain.Entities.AuthModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.QuizModel
{
    public class QuizAttempt : BaseEntity<int>
    {
        public int StudentId { get; set; }
        public int QuizId { get; set; }
        public decimal? Score { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public bool Passed { get; set; } = false;

        // Navigation
        public User Student { get; set; } = null!;
        public Quiz Quiz { get; set; } = null!;
        public ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
    }
}
