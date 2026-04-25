using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.PayoutModel
{
    /// <summary>
    /// Represents a single earning record for a teacher.
    /// Created whenever a student successfully pays for a course.
    /// One Payment → One Enrollment → One TeacherEarning.
    /// </summary>
    public class TeacherEarning : BaseEntity<int>
    {
        public string TeacherId { get; set; } = null!;
        public int CourseId { get; set; }
        public int PaymentId { get; set; }
        public int EnrollmentId { get; set; }

        // Financial snapshot at the time of the transaction
        // (rates may change later, but we keep the snapshot)
        public decimal GrossAmount { get; set; }      // What the student paid
        public decimal CommissionRate { get; set; }   // e.g., 0.80 (80%)
        public decimal NetAmount { get; set; }        // Teacher's share (GrossAmount * CommissionRate)
        public decimal PlatformFee { get; set; }      // Platform's share (GrossAmount - NetAmount)
        public string Currency { get; set; } = "EGP";

        public DateTime EarnedAt { get; set; }
        public EarningStatus Status { get; set; } = EarningStatus.Available;

        // Set once this earning is included in a monthly invoice
        public int? InvoiceId { get; set; }

        // Navigation
        public User Teacher { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public Payment Payment { get; set; } = null!;
        public Enrollment Enrollment { get; set; } = null!;
        public TeacherInvoice? Invoice { get; set; }
    }
}
