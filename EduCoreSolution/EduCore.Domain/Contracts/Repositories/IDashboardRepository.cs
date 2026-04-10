using EduCore.Shared.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IDashboardRepository
    {
        //Admin
        Task<int> GetTotalStudentsAsync(int centerId);
        Task<int> GetTotalTeachersAsync(int centerId);
        Task<int> GetTotalCoursesAsync(int centerId);
        Task<int> GetActiveCoursesAsync(int centerId);
        Task<int> GetTotalEnrollmentsAsync(int centerId);
        Task<decimal> GetTotalRevenueAsync(int centerId);
        Task<int> GetNewEnrollmentsTodayAsync(int centerId);
        Task<int> GetCertificatesIssuedAsync(int centerId);

        Task<IEnumerable<TrendPointDto>> GetEnrollmentsTrendAsync(int centerId, int days);
        Task<IEnumerable<TrendPointDto>> GetRevenueTrendAsync(int centerId, int days);
        Task<IEnumerable<TopCourseDto>> GetTopCoursesAsync(int centerId, int count);

        //Teacher 
        Task<int> GetTeacherTotalCoursesAsync(string teacherId);
        Task<int> GetTeacherPublishedCoursesAsync(string teacherId);
        Task<int> GetTeacherTotalEnrolledStudentsAsync(string teacherId);
        Task<double> GetTeacherAverageRatingAsync(string teacherId);
        Task<IEnumerable<UpcomingSessionDto>> GetTeacherUpcomingSessionsAsync(string teacherId, int count);
        Task<IEnumerable<RecentEnrollmentDto>> GetTeacherRecentEnrollmentsAsync(string teacherId, int count);

        //Student
        Task<int> GetStudentEnrolledCoursesCountAsync(string studentId);
        Task<int> GetStudentCompletedCoursesCountAsync(string studentId);
        Task<int> GetStudentCertificatesCountAsync(string studentId);
        Task<double> GetStudentOverallProgressAsync(string studentId);
        Task<IEnumerable<UpcomingSessionDto>> GetStudentUpcomingSessionsAsync(string studentId, int count);
        Task<IEnumerable<RecentQuizResultDto>> GetStudentRecentQuizResultsAsync(string studentId, int count);
    }
}
