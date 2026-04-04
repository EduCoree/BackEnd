using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.UserProfile;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IUserService
    {
        Task<Result<UserProfileDto>> GetCurrentUserAsync(string email);
        Task<Result<UserProfileDto>> UpdateProfileAsync(string email, UpdateProfileDto dto);
        Task<Result<UserProfileDto>> UpdateAvatarAsync(string email, UpdateAvatarDto dto);
        Task<Result<bool>> ChangePasswordAsync(string email, ChangePasswordDto dto);
        Task<Result<TeacherProfileDto>> GetTeacherProfileAsync(string teacherId);

    }
}
