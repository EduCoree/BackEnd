using EduCore.Domain.Entities.AuthModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;

        public AuthenticationService(UserManager<User> userManager,IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) 
                return Error.InvalidCredentials("user.InvalidCredentials");
            var IsPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if(!IsPasswordValid) 
                return Error.InvalidCredentials("user.InvalidCredentials");
            var Token = await CreatTokenAsync(user);
            return new UserDto(user.Name,user.Email,Token);


            
        }
        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                Name = registerDto.Name,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                CenterId = 11
            };
            var IdentityResult = await userManager.CreateAsync(user, registerDto.Password);

            if (IdentityResult.Succeeded) {

                var Token = await CreatTokenAsync(user);
                return new UserDto(user.Name, user.Email, Token);
            }
            return IdentityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

        }
        private async Task<string> CreatTokenAsync(User user) 
        {
            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                 new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            };
            var role = await userManager.GetRolesAsync(user);
            foreach (var r in role)
            {
                Claims.Add(new Claim(ClaimTypes.Role, r));
            }
            var secretKey = configuration["JWTOptions:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var Cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken(issuer: configuration["JWTOptions:Issuer"], 
                                             audience: configuration["JWTOptions:Audience"],
                                             claims: Claims, 
                                             expires: DateTime.Now.AddDays(7), 
                                             signingCredentials: Cred);
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
