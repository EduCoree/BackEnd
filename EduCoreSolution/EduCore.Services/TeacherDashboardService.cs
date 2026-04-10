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
    public class TeacherDashboardService : ITeacherDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public TeacherDashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<Result<TeacherDashboardDto>> GetDashboardAsync(string teacherId)
        {
            var totalCourses = await _dashboardRepository.GetTeacherTotalCoursesAsync(teacherId);
            var publishedCourses = await _dashboardRepository.GetTeacherPublishedCoursesAsync(teacherId);
            var totalEnrolledStudents = await _dashboardRepository.GetTeacherTotalEnrolledStudentsAsync(teacherId);
            var averageRating = await _dashboardRepository.GetTeacherAverageRatingAsync(teacherId);
            var upcomingSessions = await _dashboardRepository.GetTeacherUpcomingSessionsAsync(teacherId, 5);
            var recentEnrollments = await _dashboardRepository.GetTeacherRecentEnrollmentsAsync(teacherId, 10);

            return new TeacherDashboardDto(
                totalCourses,
                publishedCourses,
                totalEnrolledStudents,
                averageRating,
                upcomingSessions,
                recentEnrollments
            );
        }
    }
}

