using EduCore.Shared.Dtos.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IQuizService
    {
        Task<QuizDto> CreateQuizAsync(int courseId,CreateQuizDto request);
        Task<QuizDto> GetQuizByIdAsync(int quizId);
    }
}
