using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.DTOs.Quiz.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IAnswerOptionService
    {
        Task<AnswerOptionDto> AddAnswerOptionAsync(int courseId, int quizId, int questionId, string teacherId, CreateAnswerOptionDto request);
        Task<AnswerOptionDto> UpdateAnswerOptionAsync(int courseId, int quizId, int questionId, int optionId, string teacherId, UpdateAnswerOptionDto request);
        Task DeleteAnswerOptionAsync(int courseId, int quizId, int questionId, int optionId, string teacherId);
    }
}
