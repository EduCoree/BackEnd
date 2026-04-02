using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Identity
{
    public record UserDto(
        string Name,
        string Email,
        string Token
        );
    
}
