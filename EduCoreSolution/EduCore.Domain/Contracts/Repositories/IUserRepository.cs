using EduCore.Domain.Entities.AuthModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetTeacherWithCoursesAsync(string teacherId);
    }
}
