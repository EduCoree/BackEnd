using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    public class AuthenticationController :ApiBaseController
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
        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmail(string email)
        {
            var result=await authanticationService.CheckEmailAsync(email);
            return Ok(result);
        }
        //[Authorize]
        //[HttpGet("currenUser")]
        //public async Task<ActionResult<UserDto>> GetCurrentUser()
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    var user = await authanticationService.GetUserByEmailAsync(email);
        //    return HandleResult(user);
        //}
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<bool>> Logout()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email==null)
                return Unauthorized();

            var result = await authanticationService.LogoutAsync(email);
            return HandleResult(result);
        }

    }
}
