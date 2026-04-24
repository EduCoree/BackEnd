using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Common;
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

        public async Task<(IEnumerable<Quiz>,int totalCount)> GetQuizzesByCourseAsync(int courseId,PaginationParams pagination)
        {
            var query =  _EduCoreDbContext.Set<Quiz>()
                .Where(q => q.CourseId == courseId)
                .OrderByDescending(q => q.CreatedAt);

            var totalCount =await query.CountAsync();
            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return (items, totalCount);


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
        public async Task<IEnumerable<string>> GetAvailableCourseTitlesAsync(string studentId)
        {
            return await _EduCoreDbContext.Quizzes
                .AsNoTracking()
                .Where(q => q.IsPublished && q.Questions.Any())
                .Where(q => q.Course.Enrollments.Any(e => e.StudentId == studentId)) 
                .Where(q => q.Attempts.Count(a => a.StudentId == studentId) < q.MaxAttempts) 
                .Select(q => q.Course.Title)
                .Distinct()
                .ToListAsync();
        }
    }
}
