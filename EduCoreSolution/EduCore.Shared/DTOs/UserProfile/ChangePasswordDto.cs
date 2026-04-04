using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.UserProfile
{
    public record ChangePasswordDto(
        string CurrentPassword,
        string NewPassword
    );
}
