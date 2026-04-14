using EduCore.Shared.DTOs.EnrollmentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IEnrollmentService
    {
        // Student endpoints
        Task<EnrollmentDto> EnrollFreeAsync(string studentId, int courseId);
        Task<CheckoutResponseDto> CreateCheckoutAsync(string studentId, int courseId);

        // Admin endpoints
        Task<EnrollmentDto> RecordCashPaymentAsync(CashPaymentDto dto);

        // Webhook
        Task HandlePaymobWebhookAsync(PaymobWebhookDto webhook);
    }
}
