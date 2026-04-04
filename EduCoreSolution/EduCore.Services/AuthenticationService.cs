using EduCore.Domain.Entities.AuthModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Identity;
using EduCore.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly IEmailService _emailService;

        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration, IEmailService emailService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            _emailService = emailService;
        }


        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Error.InvalidCredentials("user.InvalidCredentials");
            var IsPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!IsPasswordValid)
                return Error.InvalidCredentials("user.InvalidCredentials");
            var Token = await CreatTokenAsync(user);
            return new UserDto(user.Name, user.Email, Token);



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

            if (IdentityResult.Succeeded)
            {

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
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                //old tokens are rejected after logout
                new Claim("securityStamp", user.SecurityStamp ?? string.Empty)

            };
            var role = await userManager.GetRolesAsync(user);
            foreach (var r in role)
            {
                Claims.Add(new Claim(ClaimTypes.Role, r));
            }
            var secretKey = configuration["JWTOptions:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var Cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken(issuer: configuration["JWTOptions:Issuer"],
                                             audience: configuration["JWTOptions:Audience"],
                                             claims: Claims,
                                             expires: DateTime.Now.AddDays(7),
                                             signingCredentials: Cred);
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }






































        public async Task<Result<string>> SendEmailConfirmationAsync(string email, string baseUrl)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return "If this email is registered, a confirmation link has been sent.";    // to prevent email enumeration attacks and make dont know the email exists in the system or not
            if (user.EmailConfirmed)
                return "If this email is registered, a confirmation link has been sent.";     
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{baseUrl}/api/auth/confirm-email?email={user.Email}&token={Uri.EscapeDataString(token)}";
            var subject = "Confirm your EduCore Email ✅";
            var htmlBody = $"""
<!DOCTYPE html>
<html>
<body style="margin:0;padding:0;background:#F0EDE8;font-family:Arial,sans-serif;">
<table width="100%" cellpadding="0" cellspacing="0" style="background:#F0EDE8;padding:32px 16px;">
  <tr><td align="center">
    <table width="520" cellpadding="0" cellspacing="0" style="max-width:520px;width:100%;">
      <tr>
        <td style="background:#1A2B2B;border-radius:10px 10px 0 0;padding:22px 32px;">
          <table cellpadding="0" cellspacing="0"><tr>
            <td style="width:8px;height:8px;background:#5EC7A2;border-radius:50%;"></td>
            <td style="padding-left:10px;color:#E8E4DC;font-size:15px;font-family:Georgia,serif;letter-spacing:0.5px;">EduCore</td>
          </tr></table>
        </td>
      </tr>
      <tr>
        <td style="background:#FDFCFA;border:1px solid #DDD9D0;border-top:none;border-radius:0 0 10px 10px;padding:40px 32px 36px;">
          <p style="font-size:11px;letter-spacing:2.5px;color:#5EC7A2;text-transform:uppercase;margin:0 0 16px;">Confirm your email</p>
          <h1 style="font-size:22px;font-weight:400;color:#1A2B2B;margin:0 0 12px;font-family:Georgia,serif;">You're almost there</h1>
          <p style="font-size:14px;color:#5A5650;margin:0 0 32px;line-height:1.7;">
            Please confirm your email address by clicking the button below.
          </p>
          <a href="{confirmationLink}"
             style="display:inline-block;background:#1A2B2B;color:#E8E4DC;text-decoration:none;font-size:14px;padding:12px 28px;border-radius:6px;margin-bottom:32px;">
            Confirm Email
          </a>
          <p style="font-size:13px;color:#5A5650;line-height:1.7;margin:0;border-top:1px solid #E8E4DC;padding-top:24px;">
            This link will expire in 24 hours. If you didn't create an account, you can safely ignore this email.
          </p>
        </td>
      </tr>
    </table>
  </td></tr>
</table>
</body>
</html>
""";

            await _emailService.SendEmailAsync(email, subject, htmlBody);
            return "Confirmation Email Sent Successfully";

        }

        public async Task<Result<string>> ConfirmEmailAsync(EmailConfirmationDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return "Invalid confirmation link.";
            if (user.EmailConfirmed)   // to make sure that the user didn't click the confirmation link more than once
                return "Email is already confirmed.";
            var decodedToken = Uri.UnescapeDataString(dto.Token);
            var result = await userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
            return "Email confirmed successfully.";

        }
        public async Task<Result<string>> SendOtpAsync(string email,OtpPurpose purpose)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return "If this email is registered, an OTP has been sent.";
            var code = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var subject = $"Your EduCore OTP Code";
            var htmlBody = $"""
<!DOCTYPE html>
<html>
<body style="margin:0;padding:0;background:#F0EDE8;font-family:Arial,sans-serif;">
<table width="100%" cellpadding="0" cellspacing="0" style="background:#F0EDE8;padding:32px 16px;">
  <tr><td align="center">
    <table width="520" cellpadding="0" cellspacing="0" style="max-width:520px;width:100%;">
      <tr>
        <td style="background:#1A2B2B;border-radius:10px 10px 0 0;padding:22px 32px;">
          <table cellpadding="0" cellspacing="0"><tr>
            <td style="width:8px;height:8px;background:#5EC7A2;border-radius:50%;"></td>
            <td style="padding-left:10px;color:#E8E4DC;font-size:15px;font-family:Georgia,serif;letter-spacing:0.5px;">EduCore</td>
          </tr></table>
        </td>
      </tr>
      <tr>
        <td style="background:#FDFCFA;border:1px solid #DDD9D0;border-top:none;border-radius:0 0 10px 10px;padding:40px 32px 36px;">
          <p style="font-size:11px;letter-spacing:2.5px;color:#5EC7A2;text-transform:uppercase;margin:0 0 16px;">Sign-in code</p>
          <h1 style="font-size:22px;font-weight:400;color:#1A2B2B;margin:0 0 12px;font-family:Georgia,serif;">Verify your identity</h1>
          <p style="font-size:14px;color:#5A5650;margin:0 0 32px;line-height:1.7;">
            Enter the code below to complete your sign-in. It's single-use and expires in 10 minutes.
          </p>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#F4F1EB;border:1px solid #DDD9D0;border-radius:8px;margin-bottom:28px;">
            <tr><td style="padding:28px;text-align:center;">
              <p style="font-size:11px;letter-spacing:2px;color:#7A7670;text-transform:uppercase;margin:0 0 12px;">One-time passcode</p>
              <div style="font-family:'Courier New',Courier,monospace;font-size:38px;font-weight:700;letter-spacing:12px;color:#1A2B2B;padding-left:12px;">{code}</div>
              <p style="font-size:12px;color:#5EC7A2;margin:14px 0 0;">Expires in 10 minutes</p>
            </td></tr>
          </table>
          <p style="font-size:13px;color:#5A5650;line-height:1.7;margin:0;border-top:1px solid #E8E4DC;padding-top:24px;">
            EduCore will never ask for this code by phone or chat. If you didn't request this, you can safely ignore this email.
          </p>
        </td>
      </tr>
    </table>
  </td></tr>
</table>
</body>
</html>
""";

            await _emailService.SendEmailAsync(email, subject, htmlBody);
            return "OTP sent successfully.";

        }

        public async Task<Result<string>> VerifyOtpAsync(VerifyOtpDto dto,OtpPurpose purpose)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Error.Validation("InvalidOTP", "Invalid email or OTP.");
            var isValid = await userManager.VerifyTwoFactorTokenAsync(user,"Email", dto.Otp);
            if (!isValid)
                return Error.Validation("InvalidOTP", "Invalid  or Expired Otp"); //
            switch(purpose)
            {
                case OtpPurpose.Password:
                    var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                    return resetToken; // return the reset token to the client to include it in the password reset request (this token valid only for password reset)

                case OtpPurpose.Email:
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    break;
            }
            return "OTP verified successfully.";


        }

        public async Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if(user == null) return Error.Validation("NotFound", "User not found.");
            var result = await userManager.ResetPasswordAsync(user, dto.ResetToken, dto.NewPassword);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code == "InvalidToken"))
                    return Error.Validation("InvalidToken", "Reset token is invalid or expired.");

                return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
            }
            return "Password reset successfully.";

        public async Task<bool> CheckEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return user != null;
        }
        //public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
        //{
        //    var user = await userManager.FindByEmailAsync(email);
        //    if (user == null)
        //        return Error.NotFound("user.NotFound",$"No user with email {email} found");
        //    return new UserDto(user.Name, user.Email,await CreatTokenAsync(user));
            
        //}

        public async Task<Result<bool>> LogoutAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("user.NotFound", $"No user with email {email} found");

            // Invalidates all existing tokens for this user
            await userManager.UpdateSecurityStampAsync(user);

            return true;
        }
    }
}
