using EduCore.Shared.DTOs.EnrollmentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface ICashPaymentRequestService
    {
        // Student
        Task<CashPaymentRequestDto> CreateRequestAsync(string studentId, int courseId);

        // Admin
        Task<IEnumerable<CashPaymentRequestDto>> GetAllRequestsAsync();
        Task<CashPaymentRequestDto> ConfirmRequestAsync(int requestId);
        Task<CashPaymentRequestDto> RejectRequestAsync(int requestId);
    }
}
