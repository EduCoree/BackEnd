using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{    public class UserRepository : IUserRepository
    {
        private readonly EduCoreDbContext context;

        public UserRepository(EduCoreDbContext context)
        {
            this.context = context;
        }

        public async Task<User?> GetTeacherWithCoursesAsync(string teacherId)
        {
            return await context.Users
                .Include(u => u.TaughtCourses)
                    .ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(u => u.Id == teacherId);
        }
    }
}
