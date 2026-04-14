using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Progress;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/teacher/progress")]
    [Authorize(Roles = "Teacher")]
    public class TeacherProgressController : ControllerBase
    {
        private readonly IProgressService _service;

        public TeacherProgressController(IProgressService service)
        {
            _service = service;
        }

        private string TeacherId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("courses/{courseId:int}/students")]
        public async Task<IActionResult> GetStudentsProgress(
            int courseId, CancellationToken ct)
        {
            var result = await _service.GetStudentsProgressAsync(TeacherId, courseId, ct);
            return Ok(ApiResponse<List<StudentProgressSummaryResponse>>.SuccessResult(result));
        }

        [HttpGet("courses/{courseId:int}/students/{studentId}")]
        public async Task<IActionResult> GetStudentDetail(
            int courseId, string studentId, CancellationToken ct)
        {
            var result = await _service.GetStudentDetailAsync(TeacherId, courseId, studentId, ct);
            return Ok(ApiResponse<StudentLessonDetailResponse>.SuccessResult(result));
        }
    }
}
