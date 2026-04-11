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
    [Route("api/student/dashboard")]
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : ApiBaseController
    {
        private readonly IStudentDashboardService _dashboardService;

        public StudentDashboardController(IStudentDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult> GetDashboard()
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
             ?? User.FindFirst("sub")?.Value;
            if (studentId is null)
                return Unauthorized();

            var result = await _dashboardService.GetDashboardAsync(studentId);
            return HandleResult(result);
        }
    }
}
