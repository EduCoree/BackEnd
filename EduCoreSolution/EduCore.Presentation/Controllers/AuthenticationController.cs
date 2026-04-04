using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Identity;
using EduCore.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService authanticationService;

        public AuthenticationController(IAuthenticationService authanticationService)
        {
            this.authanticationService = authanticationService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var result = await authanticationService.RegisterAsync(registerDto);
            return HandleResult(result);
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var result = await authanticationService.LoginAsync(loginDto);
            return HandleResult(result);
        }












































        [HttpPost("send-confirmation")]
        public async Task<ActionResult<string>> SendEmailConfirmation([FromBody] string email)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await authanticationService.SendEmailConfirmationAsync(email, baseUrl);
            return HandleResult(result);
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult<string>> ConfirmEmail([FromQuery] EmailConfirmationDto dto)
        {
            var result = await authanticationService.ConfirmEmailAsync(dto);
            return HandleResult(result);

        }
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromQuery] string email, [FromQuery] OtpPurpose purpose)
        {
            var result = await authanticationService.SendOtpAsync(email, purpose);
            return HandleResult(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto, [FromQuery] OtpPurpose purpose)
        {
            var result = await authanticationService.VerifyOtpAsync(dto, purpose);
            return HandleResult(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await authanticationService.ResetPasswordAsync(dto);
            return HandleResult(result);
        }
    }
}
