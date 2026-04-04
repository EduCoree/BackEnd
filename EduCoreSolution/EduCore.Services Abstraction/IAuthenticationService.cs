
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Identity;
using EduCore.Shared.Enums;
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



        Task<Result<string>> SendEmailConfirmationAsync(string email, string baseUrl);
        Task<Result<string>> ConfirmEmailAsync(EmailConfirmationDto dto);
        Task<Result<string>> SendOtpAsync(string email,OtpPurpose purpose);
        Task<Result<string>> VerifyOtpAsync(VerifyOtpDto dto,OtpPurpose purpose);
        Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
