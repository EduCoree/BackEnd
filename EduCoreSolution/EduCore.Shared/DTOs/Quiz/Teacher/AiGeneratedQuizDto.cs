using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Teacher
{
    public class AiGeneratedQuizDto
    {
        public List<AiGeneratedQuestionDto> Questions { get; init; } = new();
    }
}
