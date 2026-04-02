using EduCore.Shared.DTOs.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IQuestionService
    {
        Task<QuizDetailsDto> GetQuestionsByQuizAsync(int courseId, int quizId);
        Task<QuestionDto> AddQuestionAsync(int courseId, int quizId,CreateQuestionDto request);
        Task<QuestionDto> UpdateQuestionAsync(int courseId, int quizId, CreateQuestionDto request);
        Task DeleteQuestionAsync(int courseId, int quizId, int questionId);
    }
}
