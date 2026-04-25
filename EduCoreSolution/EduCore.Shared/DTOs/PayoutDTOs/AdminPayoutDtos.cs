using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.PayoutDTOs
{
    /// <summary>
    /// Admin request to mark an invoice as paid (they gave the teacher cash/bank transfer/etc).
    /// </summary>
    public class MarkInvoiceAsPaidDto
    {
        [Required]
        public PayoutMethod PayoutMethod { get; set; }

        [MaxLength(100)]
        public string? PayoutReference { get; set; }   // e.g., bank txn ref, cash receipt #

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Admin request to cancel an invoice (rare — used when there's a data error).
    /// Cancelling an invoice restores its earnings back to Available status
    /// so they can be re-invoiced later.
    /// </summary>
    public class CancelInvoiceDto
    {
        [Required, MaxLength(500)]
        public string Reason { get; set; } = null!;
    }

    /// <summary>
    /// Response when admin triggers the monthly job manually (for testing or backfill).
    /// </summary>
    public class GenerateInvoicesResultDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public int TeachersProcessed { get; set; }
        public int InvoicesCreated { get; set; }
        public int TeachersSkipped { get; set; }
        public int TeachersFailed { get; set; }
        public decimal TotalAmountGenerated { get; set; }

        public List<string> FailedTeacherIds { get; set; } = new();
        public List<string> Messages { get; set; } = new();
    }

    /// <summary>
    /// High-level admin dashboard KPIs for the payout system.
    /// </summary>
    public class AdminPayoutDashboardDto
    {
        // Pending payouts (Draft + Issued invoices)
        public decimal TotalPendingPayouts { get; set; }
        public int PendingInvoicesCount { get; set; }

        // Current month snapshot (real-time, before invoice generation)
        public decimal CurrentMonthTeacherEarnings { get; set; }
        public decimal CurrentMonthPlatformRevenue { get; set; }
        public int CurrentMonthPaidEnrollments { get; set; }

        // Last month snapshot
        public decimal LastMonthPaidToTeachers { get; set; }

        public string Currency { get; set; } = "EGP";
    }
}
