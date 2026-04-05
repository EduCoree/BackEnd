using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.DTOs.AdminUser;
using EduCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly EduCoreDbContext context;

        public AdminUserRepository(EduCoreDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<User>> GetAllTeachersAsync(string? search)
        {
            return await context.Users
                .Include(u => u.TaughtCourses)
                .Where(u => u.Role == UserRole.Teacher &&
                            (search == null || u.Name.Contains(search) || u.Email!.Contains(search)))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllStudentsAsync(string? search, bool? isActive)
        {
            return await context.Users
                .Include(u => u.Enrollments)
                .Where(u => u.Role == UserRole.Student &&
                            (search == null || u.Name.Contains(search) || u.Email!.Contains(search)) &&
                            (isActive == null || u.IsActive == isActive))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<User?> GetStudentWithDetailsAsync(string studentId)
        {
            return await context.Users
                .Include(u => u.Enrollments)
                    .ThenInclude(e => e.Course)
                .Include(u => u.Payments)
                .Include(u => u.AttendanceRecords)
                    .ThenInclude(a => a.LiveSession)
                        .ThenInclude(ls => ls.Lesson)
                .FirstOrDefaultAsync(u => u.Id == studentId);
        }

        public async Task<bool> IsEnrolledAsync(string studentId, int courseId)
        {
            return await context.Set<Enrollment>()
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task<Enrollment?> EnrollStudentAsync(string studentId, ManualEnrollDto dto)
        {
            var courseExists = await context.Courses.AnyAsync(c => c.Id == dto.CourseId);
            if (!courseExists) return null;

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = dto.CourseId,
                Type = Enum.Parse<EnrollmentType>(dto.Type),
                Status = EnrollmentStatus.Active,
                EnrolledAt = DateTime.UtcNow,
                ExpiresAt = dto.ExpiresAt
            };

            await context.Set<Enrollment>().AddAsync(enrollment);
            await context.SaveChangesAsync();

            return await context.Set<Enrollment>()
                .Include(e => e.Course)
                .FirstAsync(e => e.Id == enrollment.Id);
        }
    }
}
