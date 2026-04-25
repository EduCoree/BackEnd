using EduCore.Domain.Entities.AuthModel;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.PayoutModel
{
    /// <summary>
    /// Monthly invoice for a teacher, aggregating all earnings in the period
    /// plus any tier-based enrollment bonus.
    /// Generated automatically by a background job at the start of each month,
    /// covering the previous month's activity.
    /// </summary>
    public class TeacherInvoice : BaseEntity<int>
    {
        public string InvoiceNumber { get; set; } = null!;   // e.g., "INV-2026-04-001"
        public string TeacherId { get; set; } = null!;

        // Period this invoice covers (whole month: first day 00:00 → last day 23:59:59)
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        // Aggregates (computed when invoice is generated, frozen afterwards)
        public int PaidEnrollmentsCount { get; set; }   // Number of paid enrollments in period
        public decimal EarningsTotal { get; set; }      // Sum of NetAmount from all earnings
        public decimal TierBonus { get; set; }          // Bonus based on PaidEnrollmentsCount
        public decimal TotalAmount { get; set; }        // EarningsTotal + TierBonus

        public string Currency { get; set; } = "EGP";
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        // Payment details (filled when admin marks as Paid)
        public PayoutMethod? PayoutMethod { get; set; }
        public string? PayoutReference { get; set; }    // e.g., bank transaction ref
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        // Navigation
        public User Teacher { get; set; } = null!;
        public ICollection<TeacherEarning> Earnings { get; set; } = new List<TeacherEarning>();
    }
}
