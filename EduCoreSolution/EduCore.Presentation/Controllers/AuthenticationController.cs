using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
     }
}
