using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment, int>, IEnrollmentRepository
    {
        public EnrollmentRepository(EduCoreDbContext context) : base(context) { }

        public async Task<bool> IsEnrolledAsync(string studentId, int courseId)
        {
            return await _EduCoreDbContext.Set<Enrollment>()
                .AnyAsync(e => e.StudentId == studentId
                            && e.CourseId == courseId
                            && e.Status == EduCore.Shared.Enums.EnrollmentStatus.Active);
        }

        public async Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(string studentId)
        {
            return await _EduCoreDbContext.Set<Enrollment>()
                .AsNoTracking()
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId
                         && e.Status == EduCore.Shared.Enums.EnrollmentStatus.Active)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();
        }
    }
}
