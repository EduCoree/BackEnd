using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.AdminUse
{
    public record TeacherSummaryDto(
        string Id,
        string Name,
        string UserName,
        string Email,
        string? PhoneNumber,
        string? AvatarUrl,
        bool IsActive,
        int CourseCount,
        DateTime CreatedAt
    );
}
