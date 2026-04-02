using EduCore.Shared.Dtos.Quiz;
using EduCore.Shared.DTOs.Quiz;
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
        Task<QuizDto> GetQuizByIdAsync(int courseId,int quizId);
        Task<IEnumerable<QuizDto>> GetQuizzesByCourseAsync(int courseId);
        Task<QuizDto> UpdateQuizAsync(int courseId,int quizId, CreateQuizDto request);
        Task DeleteQuizAsync(int courseId, int quizId);
        
    }
}
