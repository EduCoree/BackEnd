using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Identity;
using EduCore.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService authanticationService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IAuthenticationService authanticationService,IConfiguration configuration)
        {
            this.authanticationService = authanticationService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterDto registerDto)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await authanticationService.RegisterAsync(registerDto,baseUrl);
            return HandleResult(result);
        }

        // POST api/authentication/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var result = await authanticationService.LoginAsync(loginDto);
            return HandleResult(result);
        }

        // POST api/authentication/refresh-token
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken(RefreshTokenDto dto)
        {
            var result = await authanticationService.RefreshTokenAsync(dto.RefreshToken);
            return HandleResult(result);
        }

        // POST api/authentication/logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<bool>> Logout(RefreshTokenDto dto)
        {
            var result = await authanticationService.LogoutAsync(dto.RefreshToken);
            return HandleResult(result);
        }


        [HttpPost("resend-confirmation")]
        public async Task<ActionResult<ResendEmailResponseDto>> ReSendEmailConfirmation(ResendEmailRequestDto resendEmailDto)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await authanticationService.SendEmailConfirmationAsync(resendEmailDto.Email, baseUrl);
            return HandleResult(result);
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult<string>> ConfirmEmail([FromQuery] EmailConfirmationDto dto)
        {
            var frontendUrl = _configuration["FrontendUrl"];
            var result = await authanticationService.ConfirmEmailAsync(dto);
            if (result.IsSuccess)
                return Redirect($"{frontendUrl}/#/confirm-email?success=true");
            var errorCode = result.Errors.FirstOrDefault()?.Code ?? "InvalidLink";
            return Redirect($"{frontendUrl}/#/confirm-email?success=false&errorCode={errorCode}");

        }
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(sendOtpDto sendOtpDto)
        {
            var result = await authanticationService.SendOtpAsync(sendOtpDto);
            return HandleResult(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var result = await authanticationService.VerifyOtpAsync(dto);
            return HandleResult(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await authanticationService.ResetPasswordAsync(dto);
            return HandleResult(result);
        }
        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmail(string email)
        {
            var result=await authanticationService.CheckEmailAsync(email);
            return Ok(result);
        }
        

    }
}
