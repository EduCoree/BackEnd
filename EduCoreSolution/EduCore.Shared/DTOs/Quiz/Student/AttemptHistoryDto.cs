using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class AttemptHistoryDto
    {
        public int AttemptId { get; init; }
        public int QuizId { get; init; }
        public string QuizTitle { get; init; } = string.Empty;
        public decimal Score { get; init; }
        public bool Passed { get; init; }
        public DateTime StartedAt { get; init; }
        public DateTime? SubmittedAt { get; init; }
    }
}
