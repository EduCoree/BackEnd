using EduCore.Domain.Contracts.Repositories;
using EduCore.Services_Abstraction;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class StudentDashboardService : IStudentDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public StudentDashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<Result<StudentDashboardDto>> GetDashboardAsync(string studentId)
        {
            var enrolledCourses = await _dashboardRepository.GetStudentEnrolledCoursesCountAsync(studentId);
            var completedCourses = await _dashboardRepository.GetStudentCompletedCoursesCountAsync(studentId);
            var certificatesEarned = await _dashboardRepository.GetStudentCertificatesCountAsync(studentId);
            var overallProgress = await _dashboardRepository.GetStudentOverallProgressAsync(studentId);
            var upcomingSessions = await _dashboardRepository.GetStudentUpcomingSessionsAsync(studentId, 5);
            var recentQuizResults = await _dashboardRepository.GetStudentRecentQuizResultsAsync(studentId, 5);

            return new StudentDashboardDto(
                enrolledCourses,
                completedCourses,
                certificatesEarned,
                overallProgress,
                upcomingSessions,
                recentQuizResults
            );
        }
    }

}
