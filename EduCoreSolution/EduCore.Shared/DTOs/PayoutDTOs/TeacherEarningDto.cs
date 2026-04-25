using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.PayoutDTOs
{
    /// <summary>
    /// A single earning record as displayed to the teacher.
    /// One earning = one paid enrollment.
    /// </summary>
    public class TeacherEarningDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = null!;
        public decimal GrossAmount { get; set; }       // What the student paid
        public decimal CommissionRate { get; set; }    // e.g., 0.80
        public decimal NetAmount { get; set; }         // Teacher's share
        public string Currency { get; set; } = "EGP";
        public DateTime EarnedAt { get; set; }
        public EarningStatus Status { get; set; }
        public int? InvoiceId { get; set; }            // null if not yet invoiced
        public string? InvoiceNumber { get; set; }     // populated if invoiced
    }
}
