using EduCore.Domain.Entities.AuthModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public readonly UserManager<User> userManager;
        public AuthenticationService(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }


        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) 
                return Error.InvalidCredentials("user.InvalidCredentials");
            var IsPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if(!IsPasswordValid) 
                return Error.InvalidCredentials("user.InvalidCredentials");

            return new UserDto(user.Name,user.Email,"Token");


            
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                Name = registerDto.Name,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                CenterId = 2
            };
            var IdentityResult = await userManager.CreateAsync(user, registerDto.Password);

            if (IdentityResult.Succeeded)
                return new UserDto(user.Name, user.Email, "Token");
            return IdentityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

        }
    }
}
