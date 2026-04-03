using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.UserProfile;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IUserRepository userRepository;

        public UserService(UserManager<User> userManager,IUserRepository userRepository)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
        }

        public async Task<Result<UserProfileDto>> GetCurrentUserAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Error.NotFound("user.NotFound", $"No user with email {email} found");

            return MapToProfileDto(user);
        }

        public async Task<Result<UserProfileDto>> UpdateProfileAsync(string email, UpdateProfileDto dto)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Error.NotFound("user.NotFound", $"No user with email {email} found");

            user.Name = dto.Name;
            user.PhoneNumber = dto.PhoneNumber;
            user.Bio = dto.Bio;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            return MapToProfileDto(user);
        }

        public async Task<Result<UserProfileDto>> UpdateAvatarAsync(string email, UpdateAvatarDto dto)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Error.NotFound("user.NotFound", $"No user with email {email} found");

            user.AvatarUrl = dto.AvatarUrl;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            return MapToProfileDto(user);
        }

        public async Task<Result<bool>> ChangePasswordAsync(string email, ChangePasswordDto dto)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Error.NotFound("user.NotFound", $"No user with email {email} found");

            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            return true;
        }

        public async Task<Result<TeacherProfileDto>> GetTeacherProfileAsync(string teacherId)
        {
            // Include TaughtCourses + their enrollments for the count
            var user = await userRepository.GetTeacherWithCoursesAsync(teacherId);

            if (user is null)
                return Error.NotFound("teacher.NotFound", $"No teacher with id {teacherId} found");

            if (user.Role != UserRole.Teacher)
                return Error.Validation("teacher.NotATeacher", $"User {teacherId} is not a teacher");

            var courses = user.TaughtCourses.Select(c => new TeacherCourseDto(
                c.Id,
                c.Title,
                c.Price,
                c.Enrollments.Count
            ));

            return new TeacherProfileDto(
                user.Id,
                user.Name,
                user.Bio,
                user.AvatarUrl,
                courses
            );
        }

        private static UserProfileDto MapToProfileDto(User user) =>
            new(
                user.Id,
                user.Name,
                user.UserName!,
                user.Email!,
                user.PhoneNumber,
                user.Bio,
                user.AvatarUrl,
                user.Role.ToString(),
                user.CreatedAt
            );
    }
}
