using EduCore.Domain.Entities.QuizModel;
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
        Task<IEnumerable<QuizAttempt>> GetStudentHistoryAsync(string studentId);
        Task<IEnumerable<QuizAttempt>> GetQuizHistoryAsync(int quizId, string studentId);

    }
}
