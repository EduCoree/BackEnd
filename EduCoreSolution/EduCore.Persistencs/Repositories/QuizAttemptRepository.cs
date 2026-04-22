using EduCore.Domain.Contracts;
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

        public async Task<(IEnumerable<QuizAttempt>,int totalCount)> GetStudentHistoryAsync(string studentId,PaginationParams pagination, HistoryFilterDto filter)
        {

            var query = _EduCoreDbContext.QuizAttempts
           .AsNoTracking()
           .Where(a => a.StudentId == studentId);

            if (!string.IsNullOrEmpty(filter.CourseTitle) && filter.CourseTitle != "All")
            {
                query = query.Where(a => a.Quiz.Course.Title == filter.CourseTitle);
            }
            if (filter.Status == "Passed")
            {
                query = query.Where(a => a.Score >= a.Quiz.PassScore); 
            }
            else if (filter.Status == "Failed")
            {
                query = query.Where(a => a.Score < a.Quiz.PassScore);
            }
            if (!string.IsNullOrEmpty(filter.DateRange) && filter.DateRange != "All")
            {
                int days = int.Parse(filter.DateRange);
                var cutoff = DateTime.UtcNow.AddDays(-days);
                query = query.Where(a => a.SubmittedAt >= cutoff || a.StartedAt >= cutoff);
            }
            var totalCount = await query.CountAsync();


            var items = await query
          .Include(a => a.Quiz).ThenInclude(q => q.Course)
          .Include(a=>a.Quiz).ThenInclude(q=>q.Questions)
          .OrderByDescending(a => a.SubmittedAt)
          .Skip((pagination.PageNumber - 1) * pagination.PageSize)
          .Take(pagination.PageSize)
          .ToListAsync();

            return(items,totalCount);

        }

      public async Task<(IEnumerable<QuizAttempt>,int totalCount)> GetQuizHistoryAsync(int quizId, string studentId,PaginationParams pagination)
        {
            var query =  _EduCoreDbContext.QuizAttempts
                .Where(q => q.QuizId == quizId && q.StudentId == studentId)
                .AsNoTracking();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pagination.PageNumber-1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
            return(items,totalCount);
        }
        public async Task<(IEnumerable<Quiz>,int totalcount)> GetAvailableQuizzesAsync(string studentId,PaginationParams pagination, string? courseTitle)
        {
            var query=  _EduCoreDbContext.Quizzes
                .Where(q => q.IsPublished && q.Questions.Any())
                .Where(q => q.Course.Enrollments
                    .Any(e => e.StudentId == studentId))
                .Where(q => q.Attempts.Count(a => a.StudentId == studentId) < q.MaxAttempts)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(courseTitle) && courseTitle != "All")
            {
                query = query.Where(q => q.Course.Title == courseTitle);
            }
            var totalCount = await query.CountAsync();
            var items = await query
              .Include(q => q.Course)
            .OrderByDescending(q => q.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

            return (items, totalCount);
        }
        public async Task<IEnumerable<string>> GetStudentAttemptedCoursesAsync(string studentId)
        {
            return await _EduCoreDbContext.QuizAttempts
                .Where(a => a.StudentId == studentId)
                .Select(a => a.Quiz.Course.Title)
                .Distinct() 
                .ToListAsync();
        }

    }
}
