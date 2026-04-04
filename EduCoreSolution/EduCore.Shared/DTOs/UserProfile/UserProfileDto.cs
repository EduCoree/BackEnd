using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.UserProfile
{
    public record UserProfileDto(
        string Id,
        string Name,
        string UserName,
        string Email,
        string? PhoneNumber,
        string? Bio,
        string? AvatarUrl,
        string Role,
        DateTime CreatedAt
    );
}
