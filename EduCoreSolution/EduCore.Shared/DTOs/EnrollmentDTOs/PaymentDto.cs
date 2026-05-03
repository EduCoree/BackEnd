using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.EnrollmentDTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? Reference { get; set; }
        public DateTime? PaidAt { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
    }
}
