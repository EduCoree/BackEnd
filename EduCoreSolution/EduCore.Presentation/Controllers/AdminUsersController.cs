using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.AdminUse;
using EduCore.Shared.DTOs.AdminUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/users")]
    public class AdminUsersController : ApiBaseController
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUsersController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpGet("teachers")]
        public async Task<ActionResult<IEnumerable<TeacherSummaryDto>>> GetTeachers([FromQuery] string? search)
        {
            var result = await _adminUserService.GetAllTeachersAsync(search);
            return HandleResult(result);
        }

        [HttpPost("teachers")]
        public async Task<ActionResult<TeacherSummaryDto>> CreateTeacher(CreateTeacherDto dto)
        {
            var result = await _adminUserService.CreateTeacherAsync(dto);
            return HandleResult(result);
        }

        [HttpPut("teachers/{id}")]
        public async Task<ActionResult<TeacherSummaryDto>> UpdateTeacher(string id, UpdateTeacherDto dto)
        {
            var result = await _adminUserService.UpdateTeacherAsync(id, dto);
            return HandleResult(result);
        }

        [HttpPut("teachers/{id}/activate")]
        public async Task<ActionResult<bool>> ActivateTeacher(string id)
        {
            var result = await _adminUserService.SetTeacherActiveAsync(id, isActive: true);
            return HandleResult(result);
        }

        [HttpPut("teachers/{id}/deactivate")]
        public async Task<ActionResult<bool>> DeactivateTeacher(string id)
        {
            var result = await _adminUserService.SetTeacherActiveAsync(id, isActive: false);
            return HandleResult(result);
        }

        [HttpGet("students")]
        public async Task<ActionResult<IEnumerable<StudentSummaryDto>>> GetStudents(
            [FromQuery] string? search,
            [FromQuery] bool? isActive)
        {
            var result = await _adminUserService.GetAllStudentsAsync(search, isActive);
            return HandleResult(result);
        }

        [HttpGet("students/{id}")]
        public async Task<ActionResult<StudentDetailDto>> GetStudent(string id)
        {
            var result = await _adminUserService.GetStudentByIdAsync(id);
            return HandleResult(result);
        }

        [HttpPut("students/{id}/activate")]
        public async Task<ActionResult<bool>> ActivateStudent(string id)
        {
            var result = await _adminUserService.SetStudentActiveAsync(id, isActive: true);
            return HandleResult(result);
        }

        [HttpPut("students/{id}/deactivate")]
        public async Task<ActionResult<bool>> DeactivateStudent(string id)
        {
            var result = await _adminUserService.SetStudentActiveAsync(id, isActive: false);
            return HandleResult(result);
        }

        [HttpPost("students/{id}/enroll")]
        public async Task<ActionResult<StudentEnrollmentDto>> EnrollStudent(string id, ManualEnrollDto dto)
        {
            var result = await _adminUserService.EnrollStudentAsync(id, dto);
            return HandleResult(result);
        }
    }

}
