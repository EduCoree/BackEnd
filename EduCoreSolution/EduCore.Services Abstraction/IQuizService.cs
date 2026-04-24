using EduCore.Shared.Common;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.DTOs.Quiz.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IQuizService
    {
        Task<QuizDto> CreateQuizAsync(int courseId, string teacherId, CreateQuizDto request);
        Task<QuizDto> GetQuizByIdAsync( int quizId, string teacherId);
        Task<PagedResult<QuizDto>> GetQuizzesByCourseAsync(int courseId, string teacherId,PaginationParams pagination);
        Task<QuizDto> UpdateQuizAsync(int quizId, string teacherId, UpdateQuizDto request);
        Task DeleteQuizAsync( int quizId, string teacherId);
        Task<QuizDto> PublishQuizAsync(int quizId, string teacherId);




    }
}
