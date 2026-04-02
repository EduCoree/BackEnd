using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IAuthenticationService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto);
         Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto);
    }
}
