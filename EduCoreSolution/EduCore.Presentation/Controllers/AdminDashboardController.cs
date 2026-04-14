using EduCore.Services_Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{

    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ApiBaseController
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult> GetDashboard()
        {
            var result = await _dashboardService.GetDashboardAsync();
            return HandleResult(result);
        }

        [HttpGet("enrollments-trend")]
        public async Task<ActionResult> GetEnrollmentsTrend([FromQuery] int days = 30)
        {
            var result = await _dashboardService.GetEnrollmentsTrendAsync(days);
            return HandleResult(result);
        }

        [HttpGet("revenue-trend")]
        public async Task<ActionResult> GetRevenueTrend([FromQuery] int days = 30)
        {
            var result = await _dashboardService.GetRevenueTrendAsync(days);
            return HandleResult(result);
        }

        [HttpGet("top-courses")]
        public async Task<ActionResult> GetTopCourses()
        {
            var result = await _dashboardService.GetTopCoursesAsync();
            return HandleResult(result);
        }
    }
}
