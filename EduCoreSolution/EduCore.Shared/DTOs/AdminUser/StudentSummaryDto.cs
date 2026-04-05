using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.AdminUser
{
    public record StudentSummaryDto(
       string Id,
       string Name,
       string UserName,
       string Email,
       string? PhoneNumber,
       string? AvatarUrl,
       bool IsActive,
       int EnrollmentCount,
       DateTime CreatedAt
   );
}
