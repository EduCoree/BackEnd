using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Shared.DTOs.AdminUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IAdminUserRepository
    {
        Task<IEnumerable<User>> GetAllTeachersAsync(string? search);
        Task<IEnumerable<User>> GetAllStudentsAsync(string? search, bool? isActive);
        Task<User?> GetStudentWithDetailsAsync(string studentId);
        Task<bool> IsEnrolledAsync(string studentId, int courseId);
        Task<Enrollment?> EnrollStudentAsync(string studentId, ManualEnrollDto dto);
    }
}
