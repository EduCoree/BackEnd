using EduCore.Shared.Common;
using EduCore.Shared.DTOs.EnrollmentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IPaymentService
    {
        // payment for student
        Task<IEnumerable<PaymentDto>> GetMyPaymentsAsync(string studentId);

        //  payments for admin
        Task<PagedResult<PaymentDto>> GetAllPaymentsAsync(PaginationParams pagination);
    }
}
