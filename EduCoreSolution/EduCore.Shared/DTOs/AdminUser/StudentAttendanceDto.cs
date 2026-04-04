using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.AdminUser
{
    public record StudentAttendanceDto(
        int Id,
        int LessonId,
        string LessonTitle,
        DateTime AttendedAt
    );
}
