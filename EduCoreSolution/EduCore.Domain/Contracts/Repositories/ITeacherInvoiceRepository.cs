using EduCore.Domain.Entities.PayoutModel;
using EduCore.Shared.Common;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    /// <summary>
    /// Repository contract for TeacherInvoice entities.
    /// One invoice is generated per teacher per calendar month,
    /// aggregating all paid enrollments and any tier bonus.
    /// </summary>
    public interface ITeacherInvoiceRepository : IGenericRepository<TeacherInvoice, int>
    {
        /// <summary>
        /// Loads an invoice with its teacher profile and all earning lines,
        /// each with its course. Used for invoice detail view and PDF rendering.
        /// </summary>
        Task<TeacherInvoice?> GetByIdWithDetailsAsync(int invoiceId);

        /// <summary>
        /// Paged list of a teacher's invoices (for the teacher dashboard).
        /// Optional status filter (e.g., only Paid ones).
        /// </summary>
        Task<PagedResult<TeacherInvoice>> GetTeacherInvoicesPagedAsync(
            string teacherId,
            InvoiceStatus? status,
            PaginationParams pagination);

        /// <summary>
        /// Paged list of all invoices across all teachers (admin view).
        /// Multiple optional filters.
        /// </summary>
        Task<PagedResult<TeacherInvoice>> GetAllInvoicesPagedAsync(
            InvoiceStatus? status,
            string? teacherId,
            DateTime? from,
            DateTime? to,
            PaginationParams pagination);

        /// <summary>
        /// Guards against duplicate invoices for the same teacher/period
        /// (also enforced by unique index at DB level).
        /// </summary>
        Task<bool> ExistsForPeriodAsync(string teacherId, DateTime periodStart, DateTime periodEnd);

        /// <summary>
        /// Generates the next sequential invoice number for a given year/month.
        /// Format: INV-YYYY-MM-NNN (e.g., INV-2026-04-001, INV-2026-04-002, ...)
        /// </summary>
        Task<string> GenerateNextInvoiceNumberAsync(int year, int month);

        /// <summary>
        /// Total amount owed to teachers across all unpaid invoices (Draft + Issued).
        /// Used by the admin dashboard "pending payouts" card.
        /// </summary>
        Task<decimal> GetTotalPendingPayoutsAsync();

        /// <summary>
        /// Sum of paid invoice totals in a period — how much was actually
        /// disbursed to teachers (admin financial report).
        /// </summary>
        Task<decimal> GetTotalPaidInPeriodAsync(DateTime from, DateTime to);
    }
}
