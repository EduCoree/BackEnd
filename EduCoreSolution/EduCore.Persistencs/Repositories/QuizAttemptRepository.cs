using EduCore.Domain.Contracts;
using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.DTOs.Quiz.Student;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class QuizAttemptRepository : GenericRepository<QuizAttempt, int>, IQuizAttemptRepository
    {
        public QuizAttemptRepository(EduCoreDbContext context):base(context)
        {
            
        }
        public async Task<int> GetAttemptCountAsync(int quizId, string studentId)
        {
            return await _EduCoreDbContext.QuizAttempts.CountAsync(a => a.QuizId == quizId && a.StudentId == studentId);
        }

        public async Task<QuizAttempt?> GetAttemptWithAnswersAsync(int attemptId)
        => await _EduCoreDbContext.Set<QuizAttempt>()
            .Include(a => a.AttemptAnswers)
                .ThenInclude(aa => aa.AnswerOption)
            .Include(a => a.AttemptAnswers)
                .ThenInclude(aa => aa.Question)
                    .ThenInclude(q => q.AnswerOptions)
            .Include(a => a.Quiz)
            .FirstOrDefaultAsync(a => a.Id == attemptId);

        public async Task<IEnumerable<QuizAttempt>> GetStudentHistoryAsync(string studentId)
        {
           return await _EduCoreDbContext.Set<QuizAttempt>()
                .Where(a => a.StudentId == studentId)
                .Include(a => a.Quiz)
                .ToListAsync();
        }

    }
}
