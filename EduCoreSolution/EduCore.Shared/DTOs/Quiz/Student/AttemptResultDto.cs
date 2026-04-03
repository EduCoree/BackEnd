using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class AttemptResultDto
    {
        public int AttemptId { get; init; }
        public decimal Score { get; init; }
        public bool Passed { get; init; }
        public DateTime SubmittedAt { get; init; }
        public int TotalPoints { get; init; }
        public int EarnedPoints { get; init; }
        public ICollection<QuestionReviewDto> Review { get; init; } = new List<QuestionReviewDto>();
    }
}
