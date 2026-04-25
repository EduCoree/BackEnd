using EduCore.Shared.Common;
using EduCore.Shared.DTOs.PayoutDTOs;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    /// <summary>
    /// Admin-facing service for managing teacher payouts.
    /// All methods here require Admin role at the controller level.
    /// </summary>
    public interface IAdminPayoutService
    {
        /// <summary>
        /// Paged list of all invoices across all teachers. Multiple filters.
        /// </summary>
        Task<PagedResult<TeacherInvoiceDto>> GetAllInvoicesAsync(
            InvoiceStatus? status,
            string? teacherId,
            DateTime? from,
            DateTime? to,
            PaginationParams pagination);

        /// <summary>
        /// Detailed view of any invoice (no ownership check — admin sees everything).
        /// </summary>
        Task<TeacherInvoiceDetailDto> GetInvoiceDetailsAsync(int invoiceId);

        /// <summary>
        /// Marks an invoice as paid (admin gave the teacher cash / bank transfer).
        /// Updates all the invoice's earnings to Status = Paid.
        /// </summary>
        Task<TeacherInvoiceDto> MarkInvoiceAsPaidAsync(
            int invoiceId,
            MarkInvoiceAsPaidDto dto,
            string adminId);

        /// <summary>
        /// Cancels an invoice (data error, wrong calculation, etc).
        /// Restores earnings back to Status = Available so they can be re-invoiced.
        /// Cannot cancel an already-paid invoice.
        /// </summary>
        Task<TeacherInvoiceDto> CancelInvoiceAsync(
            int invoiceId,
            CancelInvoiceDto dto,
            string adminId);

        /// <summary>
        /// Admin dashboard: pending payouts, this-month earnings, platform revenue.
        /// </summary>
        Task<AdminPayoutDashboardDto> GetDashboardAsync();

        /// <summary>
        /// Manually trigger invoice generation for a specific month.
        /// Useful for testing, or to backfill a month the cron job missed.
        /// </summary>
        Task<GenerateInvoicesResultDto> TriggerInvoiceGenerationAsync(int year, int month);
    }
}
