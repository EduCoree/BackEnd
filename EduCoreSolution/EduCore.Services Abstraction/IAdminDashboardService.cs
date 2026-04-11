using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IAdminDashboardService
    {
        Task<Result<AdminDashboardDto>> GetDashboardAsync();
        Task<Result<IEnumerable<TrendPointDto>>> GetEnrollmentsTrendAsync(int days);
        Task<Result<IEnumerable<TrendPointDto>>> GetRevenueTrendAsync(int days);
        Task<Result<IEnumerable<TopCourseDto>>> GetTopCoursesAsync();
    }
}
