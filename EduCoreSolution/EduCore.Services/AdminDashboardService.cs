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
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private const int DefaultCenterId = 1;

        public AdminDashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<Result<AdminDashboardDto>> GetDashboardAsync()
        {
            var totalStudents = await _dashboardRepository.GetTotalStudentsAsync(DefaultCenterId);
            var totalTeachers = await _dashboardRepository.GetTotalTeachersAsync(DefaultCenterId);
            var totalCourses = await _dashboardRepository.GetTotalCoursesAsync(DefaultCenterId);
            var activeCourses = await _dashboardRepository.GetActiveCoursesAsync(DefaultCenterId);
            var totalEnrollments = await _dashboardRepository.GetTotalEnrollmentsAsync(DefaultCenterId);
            var totalRevenue = await _dashboardRepository.GetTotalRevenueAsync(DefaultCenterId);
            var newEnrollmentsToday = await _dashboardRepository.GetNewEnrollmentsTodayAsync(DefaultCenterId);
            var certificatesIssued = await _dashboardRepository.GetCertificatesIssuedAsync(DefaultCenterId);

            return new AdminDashboardDto(
                totalStudents,
                totalTeachers,
                totalCourses,
                activeCourses,
                totalEnrollments,
                totalRevenue,
                newEnrollmentsToday,
                certificatesIssued
            );
        }

        public async Task<Result<IEnumerable<TrendPointDto>>> GetEnrollmentsTrendAsync(int days)
        {
            if (days != 30 && days != 90)
                return Error.Validation("dashboard.InvalidDays", "Days must be 30 or 90");

            var trend = await _dashboardRepository.GetEnrollmentsTrendAsync(DefaultCenterId, days);
            return trend.ToList();
        }

        public async Task<Result<IEnumerable<TrendPointDto>>> GetRevenueTrendAsync(int days)
        {
            if (days != 30 && days != 90)
                return Error.Validation("dashboard.InvalidDays", "Days must be 30 or 90");

            var trend = await _dashboardRepository.GetRevenueTrendAsync(DefaultCenterId, days);
            return trend.ToList();
        }

        public async Task<Result<IEnumerable<TopCourseDto>>> GetTopCoursesAsync()
        {
            var topCourses = await _dashboardRepository.GetTopCoursesAsync(DefaultCenterId, 5);
            return topCourses.ToList();
        }
    }
}
