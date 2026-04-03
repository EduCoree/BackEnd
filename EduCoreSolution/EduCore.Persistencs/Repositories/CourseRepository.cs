using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.CourseDTOs;
using EduCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class CourseRepository : GenericRepository<Course, int>, ICourseRepository
    {
        public CourseRepository(EduCoreDbContext context) : base(context) { }
        //GetFilteredPagedAsync
        public async Task<(IEnumerable<Course> Courses, int TotalCount)> GetFilteredPagedAsync(
            CourseFilterDto filter, PaginationParams pagination)
        {
            var query = _EduCoreDbContext.Set<Course>()
                .AsNoTracking()                    
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Where(c => c.Status == CourseStatus.Published)
                .AsQueryable();

            if (filter.CategoryId.HasValue)
                query = query.Where(c => c.CategoryId == filter.CategoryId.Value);

            if (filter.Level.HasValue)
                query = query.Where(c => c.Level == filter.Level.Value);

            if (filter.PricingType.HasValue)
                query = query.Where(c => c.PricingType == filter.PricingType.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(c => c.Title.Contains(filter.Search));

            var totalCount = await query.CountAsync();

            var courses = await query
                .OrderByDescending(c => c.CreatedAt) 
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return (courses, totalCount);
        }
        //GetWithSectionsAsync
        public async Task<Course?> GetWithSectionsAsync(int courseId)
        {
            return await _EduCoreDbContext.Set<Course>()
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Sections.OrderBy(s => s.SortOrder))
                    .ThenInclude(s => s.Lessons.OrderBy(l => l.SortOrder))
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }
        //GetByTeacherAsync
        public async Task<IEnumerable<Course>> GetByTeacherAsync(string teacherId)
        {
            return await _EduCoreDbContext.Set<Course>()
                .AsNoTracking()
                .Include(c => c.Category)
                .Where(c => c.TeacherId == teacherId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
        //HasEnrollmentsAsync
        public async Task<bool> HasEnrollmentsAsync(int courseId)
        {
            return await _EduCoreDbContext.Set<Enrollment>()
                .AnyAsync(e => e.CourseId == courseId);
        }
        //GetStudentEnrolledCoursesAsync
        public async Task<IEnumerable<StudentEnrolledCourseDto>> GetStudentEnrolledCoursesAsync(
            string studentId)
        {
            return await _EduCoreDbContext.Set<Enrollment>()
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active)
                .Select(e => new StudentEnrolledCourseDto
                {
                    CourseId = e.CourseId,
                    Title = e.Course.Title,
                    CoverImage = e.Course.CoverImage,
                    TeacherName = e.Course.Teacher.Name,
                    EnrolledAt = e.EnrolledAt,
                    TotalLessons = e.Course.Sections
                        .SelectMany(s => s.Lessons).Count(),
                    CompletedLessons = _EduCoreDbContext.Set<LessonProgress>()
                        .Count(lp => lp.StudentId == studentId
                            && lp.IsCompleted
                            && e.Course.Sections
                                .SelectMany(s => s.Lessons)
                                .Select(l => l.Id)
                                .Contains(lp.LessonId))
                })
                .ToListAsync();
        }

        public async Task<string?> GetCourseTeacherIdAsync(int courseId)
        {
            return await _EduCoreDbContext.Set<Course>()
           .Where(c => c.Id == courseId)
           .Select(c => c.TeacherId)
           .FirstOrDefaultAsync();
        }
    }
}
