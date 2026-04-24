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
         Task<Result<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto,string baseUrl);
        Task<Result<UserDto>> RefreshTokenAsync(string refreshToken);
        Task<Result<bool>> LogoutAsync(string refreshToken);
        Task<Result<ResendEmailResponseDto>> SendEmailConfirmationAsync(string email, string baseUrl);
        Task<Result<string>> ConfirmEmailAsync(EmailConfirmationDto dto);
        Task<Result<SendOtpResponseDto>> SendOtpAsync(sendOtpDto sendOtpDto);
        Task<Result<VerifyOtpResponseDto>> VerifyOtpAsync(VerifyOtpDto dto);
        Task<Result<ResetPasswordResponseDto>> ResetPasswordAsync(ResetPasswordDto dto);
        Task<bool> CheckEmailAsync(string email);
      
    }
}
