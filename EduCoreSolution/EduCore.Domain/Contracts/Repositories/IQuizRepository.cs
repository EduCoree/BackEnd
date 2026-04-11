using EduCore.Domain.Entities.QuizModel;
using EduCore.Shared.DTOs.Quiz.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IQuizRepository:IGenericRepository<Quiz,int>
    {
        Task<Quiz?> GetQuizWithDetails(int quizId);
        Task<IEnumerable<Quiz>> GetQuizzesByCourseAsync(int courseId);
        Task<QuizSummaryDto?> GetQuizSummaryAsync(int quizId, string studentId);   // for student quiz dashboard
         Task<bool> HasAttemptsAsync(int quizId);
        Task<int> GetQuizQuestionsCount(int quizId);
        Task<int> GetTotalQuestionsPoints(int quizId);

    }
}
