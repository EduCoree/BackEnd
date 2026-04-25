using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.EnrollmentDTOs
{
    public class CashPaymentRequestDto
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
    }

    public class CreateCashRequestDto
    {
        public int CourseId { get; set; }
    }
}
