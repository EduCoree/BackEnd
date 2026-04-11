using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.AdminUse;
using EduCore.Shared.DTOs.AdminUser;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IAdminUserRepository adminUserRepository;

        public AdminUserService(UserManager<User> userManager, IAdminUserRepository adminUserRepository)
        {
            this.userManager = userManager;
            this.adminUserRepository = adminUserRepository;
        }

    
        public async Task<Result<IEnumerable<TeacherSummaryDto>>> GetAllTeachersAsync(string? search)
        {
            var teachers = await adminUserRepository.GetAllTeachersAsync(search);
            return teachers.Select(MapToTeacherSummary).ToList();
        }

        public async Task<Result<TeacherSummaryDto>> CreateTeacherAsync(CreateTeacherDto dto)
        {
            var emailExists = await userManager.FindByEmailAsync(dto.Email);
            if (emailExists is not null)
                return Error.Validation("teacher.EmailTaken", $"Email {dto.Email} is already in use");

            var user = new User
            {
                Name = dto.Name,
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Role = UserRole.Teacher,
                CenterId = 11,           
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(user, dto.TempPassword);
            if (!createResult.Succeeded)
                return createResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            var roleResult = await userManager.AddToRoleAsync(user, "Teacher");
            if (!roleResult.Succeeded)
                return roleResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            return MapToTeacherSummary(user);
        }

        public async Task<Result<TeacherSummaryDto>> UpdateTeacherAsync(string teacherId, UpdateTeacherDto dto)
        {
            var user = await userManager.FindByIdAsync(teacherId);
            if (user is null)
                return Error.NotFound("teacher.NotFound", $"No teacher with id {teacherId} found");

            if (user.Role != UserRole.Teacher)
                return Error.Validation("teacher.NotATeacher", $"User {teacherId} is not a teacher");

            user.Name = dto.Name;
            user.PhoneNumber = dto.PhoneNumber;
            user.Bio = dto.Bio;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            return MapToTeacherSummary(user);
        }

        public async Task<Result<bool>> SetTeacherActiveAsync(string teacherId, bool isActive)
        {
            var user = await userManager.FindByIdAsync(teacherId);
            if (user is null)
                return Error.NotFound("teacher.NotFound", $"No teacher with id {teacherId} found");

            if (user.Role != UserRole.Teacher)
                return Error.Validation("teacher.NotATeacher", $"User {teacherId} is not a teacher");

            user.IsActive = isActive;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            return true;
        }

        public async Task<Result<IEnumerable<StudentSummaryDto>>> GetAllStudentsAsync(string? search, bool? isActive)
        {
            var students = await adminUserRepository.GetAllStudentsAsync(search, isActive);
            return students.Select(MapToStudentSummary).ToList();
        }

        public async Task<Result<StudentDetailDto>> GetStudentByIdAsync(string studentId)
        {
            var user = await adminUserRepository.GetStudentWithDetailsAsync(studentId);
            if (user is null)
                return Error.NotFound("student.NotFound", $"No student with id {studentId} found");

            if (user.Role != UserRole.Student)
                return Error.Validation("student.NotAStudent", $"User {studentId} is not a student");

            return MapToStudentDetail(user);
        }

        public async Task<Result<bool>> SetStudentActiveAsync(string studentId, bool isActive)
        {
            var user = await userManager.FindByIdAsync(studentId);
            if (user is null)
                return Error.NotFound("student.NotFound", $"No student with id {studentId} found");

            if (user.Role != UserRole.Student)
                return Error.Validation("student.NotAStudent", $"User {studentId} is not a student");

            user.IsActive = isActive;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            return true;
        }

        public async Task<Result<StudentEnrollmentDto>> EnrollStudentAsync(string studentId, ManualEnrollDto dto)
        {
            var user = await userManager.FindByIdAsync(studentId);
            if (user is null)
                return Error.NotFound("student.NotFound", $"No student with id {studentId} found");

            if (user.Role != UserRole.Student)
                return Error.Validation("student.NotAStudent", $"User {studentId} is not a student");

            var alreadyEnrolled = await adminUserRepository.IsEnrolledAsync(studentId, dto.CourseId);
            if (alreadyEnrolled)
                return Error.Validation("enrollment.AlreadyEnrolled", "Student is already enrolled in this course");

            var enrollment = await adminUserRepository.EnrollStudentAsync(studentId, dto);
            if (enrollment is null)
                return Error.NotFound("course.NotFound", $"No course with id {dto.CourseId} found");

            return new StudentEnrollmentDto(
                enrollment.Id,
                enrollment.CourseId,
                enrollment.Course.Title,
                enrollment.Type.ToString(),
                enrollment.Status.ToString(),
                enrollment.EnrolledAt,
                enrollment.ExpiresAt
            );
        }

        #region Mapping methods
        private static TeacherSummaryDto MapToTeacherSummary(User u) =>
            new(u.Id, u.Name, u.UserName!, u.Email!, u.PhoneNumber,
                u.AvatarUrl, u.IsActive, u.TaughtCourses.Count, u.CreatedAt);

        private static StudentSummaryDto MapToStudentSummary(User u) =>
            new(u.Id, u.Name, u.UserName!, u.Email!, u.PhoneNumber,
                u.AvatarUrl, u.IsActive, u.Enrollments.Count, u.CreatedAt);

        private static StudentDetailDto MapToStudentDetail(User u) =>
            new(
                u.Id, u.Name, u.UserName!, u.Email!, u.PhoneNumber,
                u.Bio, u.AvatarUrl, u.IsActive, u.CreatedAt,
                u.Enrollments.Select(e => new StudentEnrollmentDto(
                    e.Id, e.CourseId, e.Course.Title,
                    e.Type.ToString(), e.Status.ToString(),
                    e.EnrolledAt, e.ExpiresAt)),
                u.Payments.Select(p => new StudentPaymentDto(
                    p.Id, p.Amount, p.Currency,
                    p.Method.ToString(), p.Status.ToString(),
                    p.Reference, p.PaidAt)),
                u.AttendanceRecords.Select(a => new StudentAttendanceDto(
                    a.Id, a.LiveSessionId, a.LiveSession.Lesson.Title, a.JoinedAt))
            );
        #endregion
    }
}
