using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.QuizModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.QuizModel
{
    public class Quiz : BaseEntity<int>
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public int? TimeLimitMins { get; set; }
        public int PassScore { get; set; }
        public int MaxAttempts { get; set; } = 1;
        public bool IsRandomized { get; set; } = false;
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Course Course { get; set; } = null!;
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
    }
}
