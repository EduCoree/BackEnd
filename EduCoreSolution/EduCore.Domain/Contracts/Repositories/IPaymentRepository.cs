using EduCore.Domain.Entities.EnrollmentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment, int>
    {
        Task<IEnumerable<Payment>> GetStudentPaymentsAsync(string studentId);
        Task<Payment?> GetByReferenceAsync(string reference);
        IQueryable<Payment> GetAllWithDetailsAsQueryable();
    }
}
