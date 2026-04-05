using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.AdminUser
{
    public record StudentEnrollmentDto(
       int Id,
       int CourseId,
       string CourseTitle,
       string Type,
       string Status,
       DateTime EnrolledAt,
       DateTime? ExpiresAt
   );
}
