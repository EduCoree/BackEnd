using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    /// <summary>
    /// Generates monthly teacher invoices by aggregating Available earnings
    /// and computing the tier-based enrollment bonus.
    /// Used by the monthly Hangfire job AND by the admin (manual regeneration).
    /// </summary>
    public interface IInvoiceGenerationService
    {
        /// <summary>
        /// Generates invoices for the previous calendar month.
        /// Intended to be called from the Hangfire job on day 1 of each month.
        /// Example: called on 2026-04-01, generates invoices for the 2026-03-01..2026-03-31 period.
        /// </summary>
        Task<InvoiceGenerationResult> GenerateForPreviousMonthAsync();

        /// <summary>
        /// Generates invoices for a specific year/month. Used by admin for
        /// manual re-runs or to backfill missed months.
        /// Idempotent: if a teacher already has an invoice for this period,
        /// they're skipped (no duplicates).
        /// </summary>
        Task<InvoiceGenerationResult> GenerateForMonthAsync(int year, int month);
    }

    /// <summary>
    /// Summary of what the generation run did. Used for logging and the
    /// admin's "run the job manually" response.
    /// </summary>
    public class InvoiceGenerationResult
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public int TeachersProcessed { get; set; }
        public int InvoicesCreated { get; set; }
        public int TeachersSkipped { get; set; }   // already had an invoice
        public int TeachersFailed { get; set; }    // error thrown — see FailedTeacherIds
        public decimal TotalAmountGenerated { get; set; }

        public List<string> FailedTeacherIds { get; set; } = new();
        public List<string> Messages { get; set; } = new();
    }
}
