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
    public class PaymentRepository : GenericRepository<Payment, int>, IPaymentRepository
    {
        public PaymentRepository(EduCoreDbContext context) : base(context) { }

        public async Task<IEnumerable<Payment>> GetStudentPaymentsAsync(string studentId)
        {
            return await _EduCoreDbContext.Set<Payment>()
                .AsNoTracking()
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Course)
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetByReferenceAsync(string reference)
        {
            return await _EduCoreDbContext.Set<Payment>()
                .FirstOrDefaultAsync(p => p.Reference == reference);
        }
    }
}
