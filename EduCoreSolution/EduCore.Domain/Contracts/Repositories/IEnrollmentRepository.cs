using EduCore.Domain.Entities.EnrollmentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment, int>
    {
        Task<bool> IsEnrolledAsync(string studentId, int courseId);
        Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(string studentId);
        Task<IEnumerable<string>> GetActiveStudentIdsByCourseAsync(int courseId);
    }
}
