using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class ReviewRepository : GenericRepository<CourseReview, int>, IReviewRepository // ← CourseReview
    {
        public ReviewRepository(EduCoreDbContext context) : base(context) { }

        public async Task<IEnumerable<CourseReview>> GetReviewsByTeacherAsync(
            string teacherId, int? courseId = null, int? minRating = null)
        {
            var query = _EduCoreDbContext.CourseReviews 
                .AsNoTracking()
                .Include(r => r.Course)
                .Include(r => r.Student)
                .Where(r => r.Course.TeacherId == teacherId);

            if (courseId.HasValue)
                query = query.Where(r => r.CourseId == courseId.Value);

            if (minRating.HasValue)
                query = query.Where(r => r.Rating >= minRating.Value);

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseReview>> GetReviewsByStudentAsync(string studentId)
        {
            return await _EduCoreDbContext.CourseReviews 
                .AsNoTracking()
                .Include(r => r.Course)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
