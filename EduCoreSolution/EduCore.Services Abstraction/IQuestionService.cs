using EduCore.Shared.DTOs.Quiz.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IQuestionService
    {
        Task<QuizDetailsDto> GetQuestionsByQuizAsync(int quizId, string teacherId);
        Task<QuestionDto> AddQuestionAsync(int quizId, string teacherId, CreateQuestionDto request);
        Task<QuestionDto> UpdateQuestionAsync(int quizId, int questionId, string teacherId, UpdateQuestionDto request);
        Task DeleteQuestionAsync(int quizId, int questionId, string teacherId);
    }
}
