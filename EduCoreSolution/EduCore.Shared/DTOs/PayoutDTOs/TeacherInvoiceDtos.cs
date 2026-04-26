using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.PayoutDTOs
{
    /// <summary>
    /// Compact invoice representation for list views.
    /// Same shape for both the teacher's "my invoices" list
    /// and the admin's "all invoices" table.
    /// </summary>
    public class TeacherInvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = null!;

        // Period
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        // Teacher info (useful for admin view — ignored by AutoMapper when teacher not loaded)
        public string TeacherId { get; set; } = null!;
        public string? TeacherName { get; set; }

        // Financial summary
        public int PaidEnrollmentsCount { get; set; }
        public decimal EarningsTotal { get; set; }
        public decimal TierBonus { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "EGP";

        public InvoiceStatus Status { get; set; }

        // Payment info (null if not paid yet)
        public PayoutMethod? PayoutMethod { get; set; }
        public string? PayoutReference { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    /// <summary>
    /// Full invoice with all earning lines — for invoice detail page.
    /// </summary>
    public class TeacherInvoiceDetailDto : TeacherInvoiceDto
    {
        public string? Notes { get; set; }
        public List<TeacherEarningDto> Earnings { get; set; } = new();
    }
}
