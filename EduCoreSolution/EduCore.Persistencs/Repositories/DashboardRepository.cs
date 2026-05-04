using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.ContentModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.DTOs.Dashboard;
using EduCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace EduCore.Persistencs.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly EduCoreDbContext _context;

    // Cairo timezone — sessions are stored in Cairo local time
    private static TimeZoneInfo GetCairoTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); }
        catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo"); }
    }

    private static readonly TimeZoneInfo CairoTz = GetCairoTimeZone();
    private static DateTime CairoNow =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CairoTz);

    public DashboardRepository(EduCoreDbContext context)
    {
        _context = context;
    }

    //  ADMIN
    

    public async Task<int> GetTotalStudentsAsync(int centerId)
    {
        return await _context.Users
            .Where(u => u.CenterId == centerId && u.Role == UserRole.Student)
            .CountAsync();
    }

    public async Task<int> GetTotalTeachersAsync(int centerId)
    {
        return await _context.Users
            .Where(u => u.CenterId == centerId && u.Role == UserRole.Teacher)
            .CountAsync();
    }

    public async Task<int> GetTotalCoursesAsync(int centerId)
    {
        return await _context.Set<Course>()
            .Where(c => c.Teacher.CenterId == centerId)
            .CountAsync();
    }

    public async Task<int> GetActiveCoursesAsync(int centerId)
    {
        return await _context.Set<Course>()
            .Where(c => c.Teacher.CenterId == centerId && c.Status == CourseStatus.Published)
            .CountAsync();
    }

    public async Task<int> GetTotalEnrollmentsAsync(int centerId)
    {
        return await _context.Set<Enrollment>()
            .Where(e => e.Student.CenterId == centerId)
            .CountAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(int centerId)
    {
        
        return await _context.Set<Payment>()
            .Where(p => p.Student.CenterId == centerId
                     && p.Status == PaymentStatus.Completed)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;
    }

    public async Task<int> GetNewEnrollmentsTodayAsync(int centerId)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Set<Enrollment>()
            .Where(e => e.Student.CenterId == centerId
                     && e.EnrolledAt >= today)
            .CountAsync();
    }

    public async Task<int> GetCertificatesIssuedAsync(int centerId)
    {
        return await _context.Set<Certificate>()
            .Where(c => c.Student.CenterId == centerId)
            .CountAsync();
    }

    //public async Task<IEnumerable<TrendPointDto>> GetEnrollmentsTrendAsync(int centerId, int days)
    //{
    //    var startDate = DateTime.UtcNow.Date.AddDays(-days);

    //    var data = await _context.Set<Enrollment>()
    //        .Where(e => e.Student.CenterId == centerId && e.EnrolledAt >= startDate)
    //        .GroupBy(e => e.EnrolledAt.Date)
    //        .Select(g => new TrendPointDto(
    //            DateOnly.FromDateTime(g.Key),
    //            g.Count()
    //        ))
    //        .OrderBy(t => t.Date)
    //        .ToListAsync();

    //    return data;
    //}
    public async Task<IEnumerable<TrendPointDto>> GetEnrollmentsTrendAsync(int centerId, int days)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);

        var data = await _context.Set<Enrollment>()
            .Where(e => e.Student.CenterId == centerId && e.EnrolledAt >= startDate)
            .GroupBy(e => e.EnrolledAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return data.Select(x => new TrendPointDto(DateOnly.FromDateTime(x.Date), x.Count));
    }
    //public async Task<IEnumerable<TrendPointDto>> GetRevenueTrendAsync(int centerId, int days)
    //{
    //    var startDate = DateTime.UtcNow.Date.AddDays(-days);

    //    var data = await _context.Set<Payment>()
    //        .Where(p => p.Student.CenterId == centerId
    //                 && p.Status == PaymentStatus.Completed
    //                 && p.PaidAt.HasValue
    //                 && p.PaidAt.Value >= startDate)
    //        .GroupBy(p => p.PaidAt!.Value.Date)
    //        .Select(g => new TrendPointDto(
    //            DateOnly.FromDateTime(g.Key),
    //            g.Sum(p => p.Amount)
    //        ))
    //        .OrderBy(t => t.Date)
    //        .ToListAsync();

    //    return data;
    //}
    public async Task<IEnumerable<TrendPointDto>> GetRevenueTrendAsync(int centerId, int days)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);

        var data = await _context.Set<Payment>()
            .Where(p => p.Student.CenterId == centerId
                     && p.Status == PaymentStatus.Completed
                     && p.PaidAt.HasValue
                     && p.PaidAt.Value >= startDate)
            .GroupBy(p => p.PaidAt!.Value.Date)
            .Select(g => new { Date = g.Key, Total = g.Sum(p => p.Amount) })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return data.Select(x => new TrendPointDto(DateOnly.FromDateTime(x.Date), x.Total));
    }
    public async Task<IEnumerable<TopCourseDto>> GetTopCoursesAsync(int centerId, int count)
    {
        return await _context.Set<Course>()
            .Where(c => c.Teacher.CenterId == centerId
                     && c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.Enrollments.Count)
            .Take(count)
            .Select(c => new TopCourseDto(
                c.Id,
                c.Title,
                c.CoverImage,
                c.Teacher.Name,
                c.Enrollments.Count
            ))
            .ToListAsync();
    }

    //  TEACHER

    public async Task<int> GetTeacherTotalCoursesAsync(string teacherId)
    {
        return await _context.Set<Course>()
            .Where(c => c.TeacherId == teacherId)
            .CountAsync();
    }

    public async Task<int> GetTeacherPublishedCoursesAsync(string teacherId)
    {
        return await _context.Set<Course>()
            .Where(c => c.TeacherId == teacherId && c.Status == CourseStatus.Published)
            .CountAsync();
    }

    public async Task<int> GetTeacherTotalEnrolledStudentsAsync(string teacherId)
    {
        return await _context.Set<Enrollment>()
            .Where(e => e.Course.TeacherId == teacherId)
            .CountAsync();
    }

    public async Task<double> GetTeacherAverageRatingAsync(string teacherId)
    {
        var avg = await _context.Set<CourseReview>()
            .Where(r => r.Course.TeacherId == teacherId)
            .AverageAsync(r => (double?)r.Rating);

        return Math.Round(avg ?? 0, 1);
    }

    public async Task<IEnumerable<UpcomingSessionDto>> GetTeacherUpcomingSessionsAsync(
        string teacherId, int count)
    {
        var now = CairoNow;

        return await _context.Set<LiveSession>()
            .Where(s => s.Lesson.Section.Course.TeacherId == teacherId
                     && s.ScheduledAt > now)
            .OrderBy(s => s.ScheduledAt)
            .Take(count)
            .Select(s => new UpcomingSessionDto(
                s.Id,
                s.Lesson.Title,
                s.Lesson.Section.Course.Title,
                s.ScheduledAt,
                s.MeetingUrl
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<RecentEnrollmentDto>> GetTeacherRecentEnrollmentsAsync(
        string teacherId, int count)
    {
        return await _context.Set<Enrollment>()
            .Where(e => e.Course.TeacherId == teacherId)
            .OrderByDescending(e => e.EnrolledAt)
            .Take(count)
            .Select(e => new RecentEnrollmentDto(
                e.Id,
                e.Student.Name,
                e.Student.AvatarUrl,
                e.Course.Title,
                e.EnrolledAt
            ))
            .ToListAsync();
    }

    //  STUDENT

    public async Task<int> GetStudentEnrolledCoursesCountAsync(string studentId)
    {
        return await _context.Set<Enrollment>()
            .Where(e => e.StudentId == studentId)
            .CountAsync();
    }

    public async Task<int> GetStudentCompletedCoursesCountAsync(string studentId)
    {
        return await _context.Set<Enrollment>()
              .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Completed)
               .CountAsync();
    }

    public async Task<int> GetStudentCertificatesCountAsync(string studentId)
    {
        return await _context.Set<Certificate>()
            .Where(c => c.StudentId == studentId)
            .CountAsync();
    }

    public async Task<double> GetStudentOverallProgressAsync(string studentId)
    {
        // Get all courses the student is enrolled in
        var enrolledCourseIds = await _context.Set<Enrollment>()
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId)
            .ToListAsync();

        if (!enrolledCourseIds.Any())
            return 0;

        // Total lessons across enrolled courses
        var totalLessons = await _context.Set<Lesson>()
            .Where(l => enrolledCourseIds.Contains(l.Section.CourseId))
            .CountAsync();

        if (totalLessons == 0)
            return 0;

        // Completed lessons
        var completedLessons = await _context.Set<LessonProgress>()
            .Where(lp => lp.StudentId == studentId
                      && lp.IsCompleted
                      && enrolledCourseIds.Contains(lp.Lesson.Section.CourseId))
            .CountAsync();

        return Math.Round((double)completedLessons / totalLessons * 100, 1);
    }

    public async Task<IEnumerable<UpcomingSessionDto>> GetStudentUpcomingSessionsAsync(
        string studentId, int count)
    {
        var now = CairoNow;

        var enrolledCourseIds = await _context.Set<Enrollment>()
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId)
            .ToListAsync();

        return await _context.Set<LiveSession>()
            .Where(s => enrolledCourseIds.Contains(s.Lesson.Section.CourseId)
                     && s.ScheduledAt > now)
            .OrderBy(s => s.ScheduledAt)
            .Take(count)
            .Select(s => new UpcomingSessionDto(
                s.Id,
                s.Lesson.Title,
                s.Lesson.Section.Course.Title,
                s.ScheduledAt,
                s.MeetingUrl
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<RecentQuizResultDto>> GetStudentRecentQuizResultsAsync(
        string studentId, int count)
    {
        return await _context.Set<QuizAttempt>()
            .Where(a => a.StudentId == studentId && a.SubmittedAt.HasValue)
            .OrderByDescending(a => a.SubmittedAt)
            .Take(count)
            .Select(a => new RecentQuizResultDto(
                a.Id,
                a.Quiz.Title,
                a.Quiz.Course.Title,
                a.Score,
                a.Passed,
                a.SubmittedAt
            ))
            .ToListAsync();
    }
}