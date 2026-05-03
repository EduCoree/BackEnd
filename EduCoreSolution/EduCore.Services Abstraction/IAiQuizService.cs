using EduCore.Shared.DTOs.Quiz.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IAiQuizService
    {
        Task<AiGeneratedQuizDto> GenerateQuizAsync(int quizId,string teacherId,GenerateQuizAiRequest request);

        Task<AiGeneratedQuizDto> SaveGeneratedQuizAsync(int quizId,string teacherId,AiGeneratedQuizDto generated);
    }
}
