using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.UserProfile;
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
    public class UsersController : ApiBaseController
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        // GET api/users/me
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await userService.GetCurrentUserAsync(email!);
            return HandleResult(result);
        }

        // PUT api/users/me
        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile(UpdateProfileDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await userService.UpdateProfileAsync(email!, dto);
            return HandleResult(result);
        }

        // PUT api/users/me/avatar
        [Authorize]
        [HttpPut("me/avatar")]
        public async Task<ActionResult<UserProfileDto>> UpdateAvatar(UpdateAvatarDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await userService.UpdateAvatarAsync(email!, dto);
            return HandleResult(result);
        }

        // PUT api/users/me/password
        [Authorize]
        [HttpPut("me/password")]
        public async Task<ActionResult<bool>> ChangePassword(ChangePasswordDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await userService.ChangePasswordAsync(email!, dto);
            return HandleResult(result);
        }

        // GET api/users/teachers/{id}/profile
        [HttpGet("teachers/{id}/profile")]
        public async Task<ActionResult<TeacherProfileDto>> GetTeacherProfile(string id)
        {
            var result = await userService.GetTeacherProfileAsync(id);
            return HandleResult(result);
        }
    }
}
