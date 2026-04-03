using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class AttemptDto
    {
        public int Id { get; init; }
        public int QuizId { get; init; }
        public DateTime StartedAt { get; init; }
    }
}
