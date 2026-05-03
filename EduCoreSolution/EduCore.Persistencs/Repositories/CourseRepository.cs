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
                .Include(c => c.Enrollments)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .Where(c => c.Status == CourseStatus.Published)
                .AsSplitQuery()
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
        public async Task<(IEnumerable<Course> Courses, int TotalCount)> GetAdminFilteredPagedAsync(
            CourseFilterDto filter, PaginationParams pagination)
        {
            var query = _EduCoreDbContext.Set<Course>()
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .AsSplitQuery()
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
                        .ThenInclude(l => l.VideoLesson)
                .Include(c => c.Sections.OrderBy(s => s.SortOrder))
                    .ThenInclude(s => s.Lessons.OrderBy(l => l.SortOrder))
                        .ThenInclude(l => l.PdfLesson)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }
        //GetByTeacherAsync
        public async Task<IEnumerable<Course>> GetByTeacherAsync(string teacherId)
        {
            return await _EduCoreDbContext.Set<Course>()
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .Where(c => c.TeacherId == teacherId)
                .OrderByDescending(c => c.CreatedAt)
                .AsSplitQuery()
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
            // Step 1: fetch enrollments with course/section/lesson data
            var enrollments = await _EduCoreDbContext.Set<Enrollment>()
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Teacher)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Sections)
                        .ThenInclude(s => s.Lessons)
                .AsSplitQuery()
                .ToListAsync();

            if (!enrollments.Any())
                return Enumerable.Empty<StudentEnrolledCourseDto>();

            // Step 2: collect all lesson IDs the student has completed
            var allLessonIds = enrollments
                .SelectMany(e => e.Course.Sections)
                .SelectMany(s => s.Lessons)
                .Select(l => l.Id)
                .Distinct()
                .ToList();

            var completedLessonIds = await _EduCoreDbContext.Set<LessonProgress>()
                .AsNoTracking()
                .Where(lp => lp.StudentId == studentId
                          && lp.IsCompleted
                          && allLessonIds.Contains(lp.LessonId))
                .Select(lp => lp.LessonId)
                .ToListAsync();

            var completedSet = completedLessonIds.ToHashSet();

            // Step 3: project into DTOs in memory
            return enrollments.Select(e =>
            {
                var lessonIds = e.Course.Sections
                    .SelectMany(s => s.Lessons)
                    .Select(l => l.Id)
                    .ToList();

                return new StudentEnrolledCourseDto
                {
                    CourseId    = e.CourseId,
                    Title       = e.Course.Title,
                    CoverImage  = e.Course.CoverImage,
                    TeacherName = e.Course.Teacher?.Name ?? string.Empty,
                    EnrolledAt  = e.EnrolledAt,
                    TotalLessons     = lessonIds.Count,
                    CompletedLessons = lessonIds.Count(id => completedSet.Contains(id))
                };
            }).ToList();
        }

        public async Task<string?> GetCourseTeacherIdAsync(int courseId)
        {
            return await _EduCoreDbContext.Set<Course>()
           .Where(c => c.Id == courseId)
           .Select(c => c.TeacherId)
           .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesWithTeacherAsync(IEnumerable<int> courseIds)
        {
            return await _EduCoreDbContext.Set<Course>()
                .AsNoTracking()
                .Include(c => c.Teacher)
                .Where(c => courseIds.Contains(c.Id))
                .ToListAsync();
        }
    }
}
