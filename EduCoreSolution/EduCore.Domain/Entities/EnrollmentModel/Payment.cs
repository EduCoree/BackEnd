using EduCore.Domain.Entities.AuthModel;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.EnrollmentModel
{
    

    public class Payment : BaseEntity<int>
    {
        public int? EnrollmentId { get; set; }
        public string StudentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? Reference { get; set; }
        public DateTime? PaidAt { get; set; }

        // Navigation
        public Enrollment? Enrollment { get; set; } = null!;
        public User Student { get; set; } = null!;
    }
}
