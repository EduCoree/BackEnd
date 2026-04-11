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
    public class QuizRepository : GenericRepository<Quiz, int>, IQuizRepository
    {
        public QuizRepository(EduCoreDbContext context) : base(context)
        {
        }
        public async Task<Quiz?> GetQuizWithDetails(int quizId)
        {
            return await _EduCoreDbContext.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == quizId);
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByCourseAsync(int courseId)
        {
            return await _EduCoreDbContext.Set<Quiz>()
                .Where(q => q.CourseId == courseId)
                .ToListAsync();
        }
        public async Task<QuizSummaryDto?> GetQuizSummaryAsync(int quizId, string studentId)
        {
            return await _EduCoreDbContext.Quizzes
                .Where(q => q.Id == quizId)
                .Select(q => new QuizSummaryDto
                {
                    Id = q.Id,
                    courseId=q.CourseId,
                    Title = q.Title,
                    TimeLimitMins = q.TimeLimitMins,
                    PassScore = q.PassScore,
                    MaxAttempts = q.MaxAttempts,
                    TotalPoints = q.Questions.Sum(x => x.Points),
                    QuestionCount = q.Questions.Count(),
                    AttemptsLeft = q.MaxAttempts - q.Attempts.Count(a => a.StudentId == studentId)
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> HasAttemptsAsync(int quizId)
        {
            return await _EduCoreDbContext.Set<QuizAttempt>()
                .AnyAsync(a => a.QuizId == quizId);
        }
        public async Task<int> GetQuizQuestionsCount(int quizId)
        {
            return await _EduCoreDbContext.Questions.CountAsync(q => q.QuizId == quizId);
        }

        public async Task<int> GetTotalQuestionsPoints(int quizId)
        {
            return await _EduCoreDbContext.Questions.Where(q=>quizId==q.QuizId).SumAsync(q=>q.Points);
        }
    }
}
