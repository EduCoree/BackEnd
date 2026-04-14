using EduCore.Services_Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [Route("api/teacher/dashboard")]
    [Authorize(Roles = "Teacher")]
    public class TeacherDashboardController : ApiBaseController
    {
        private readonly ITeacherDashboardService _dashboardService;

        public TeacherDashboardController(ITeacherDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult> GetDashboard()
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
             ?? User.FindFirst("sub")?.Value;
            if (teacherId is null)
                return Unauthorized();

            var result = await _dashboardService.GetDashboardAsync(teacherId);
            return HandleResult(result);
        }
    }
}
