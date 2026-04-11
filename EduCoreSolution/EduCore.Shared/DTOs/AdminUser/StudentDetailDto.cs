using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.AdminUser
{
    public record StudentDetailDto(
        string Id,
        string Name,
        string UserName,
        string Email,
        string? PhoneNumber,
        string? Bio,
        string? AvatarUrl,
        bool IsActive,
        DateTime CreatedAt,
        IEnumerable<StudentEnrollmentDto> Enrollments,
        IEnumerable<StudentPaymentDto> Payments,
        IEnumerable<StudentAttendanceDto> Attendance
    );
}
