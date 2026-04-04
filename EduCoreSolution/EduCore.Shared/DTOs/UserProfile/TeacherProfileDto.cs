using EduCore.Shared.DTOs.CourseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.UserProfile
{
    public record TeacherProfileDto(
        string Id,
        string Name,
        string? Bio,
        string? AvatarUrl,
        IEnumerable<TeacherCourseDto> Courses
    );
}
