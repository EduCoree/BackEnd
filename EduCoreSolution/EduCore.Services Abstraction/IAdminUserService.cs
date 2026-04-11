using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.AdminUse;
using EduCore.Shared.DTOs.AdminUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IAdminUserService
    {
        //Teachers 
        Task<Result<IEnumerable<TeacherSummaryDto>>> GetAllTeachersAsync(string? search);
        Task<Result<TeacherSummaryDto>> CreateTeacherAsync(CreateTeacherDto dto);
        Task<Result<TeacherSummaryDto>> UpdateTeacherAsync(string teacherId, UpdateTeacherDto dto);
        Task<Result<bool>> SetTeacherActiveAsync(string teacherId, bool isActive);

        //Students 
        Task<Result<IEnumerable<StudentSummaryDto>>> GetAllStudentsAsync(string? search, bool? isActive);
        Task<Result<StudentDetailDto>> GetStudentByIdAsync(string studentId);
        Task<Result<bool>> SetStudentActiveAsync(string studentId, bool isActive);
        Task<Result<StudentEnrollmentDto>> EnrollStudentAsync(string studentId, ManualEnrollDto dto);
    }
}
