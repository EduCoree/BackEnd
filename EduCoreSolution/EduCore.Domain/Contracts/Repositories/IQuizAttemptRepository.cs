using EduCore.Domain.Entities.QuizModel;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.Quiz.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IQuizAttemptRepository:IGenericRepository<QuizAttempt,int>
    {
        Task<int> GetAttemptCountAsync(int quizId, string studentId);
        Task<QuizAttempt?> GetAttemptWithAnswersAsync(int attemptId);
        Task<(IEnumerable<QuizAttempt>, int totalCount)> GetStudentHistoryAsync(string studentId,PaginationParams pagination,HistoryFilterDto filter);
        Task<(IEnumerable<QuizAttempt>, int totalCount)> GetQuizHistoryAsync(int quizId, string studentId,PaginationParams pagination);
        Task<(IEnumerable<Quiz>, int totalcount)> GetAvailableQuizzesAsync(string studentId,PaginationParams paginationParams, string? courseTitle);
        Task<IEnumerable<string>> GetStudentAttemptedCoursesAsync(string studentId);

    }
}
