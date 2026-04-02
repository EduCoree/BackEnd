using EduCore.Shared.Dtos.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz
{
    public class QuizDetailsDto:QuizDto
    {
        public ICollection<QuestionDto> Questions { get; init; } = new List<QuestionDto>();
    }

}
