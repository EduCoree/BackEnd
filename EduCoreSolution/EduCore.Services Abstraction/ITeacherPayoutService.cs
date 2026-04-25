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
    /// Teacher-facing service for viewing earnings and invoices.
    /// Every method enforces that the teacher can only see their own data.
    /// </summary>
    public interface ITeacherPayoutService
    {
        /// <summary>
        /// Paged list of the teacher's earnings, optionally filtered by date range.
        /// </summary>
        Task<PagedResult<TeacherEarningDto>> GetMyEarningsAsync(
            string teacherId,
            DateTime? from,
            DateTime? to,
            PaginationParams pagination);

        /// <summary>
        /// Real-time preview of this month's earnings (before invoice generation).
        /// Shows "what the invoice WOULD look like if the month ended today".
        /// </summary>
        Task<CurrentMonthEarningsDto> GetCurrentMonthPreviewAsync(string teacherId);

        /// <summary>
        /// Lifetime earnings summary (total earned, paid, pending).
        /// </summary>
        Task<TeacherEarningsSummaryDto> GetEarningsSummaryAsync(string teacherId);

        /// <summary>
        /// Paged list of the teacher's invoices, optionally filtered by status.
        /// </summary>
        Task<PagedResult<TeacherInvoiceDto>> GetMyInvoicesAsync(
            string teacherId,
            InvoiceStatus? status,
            PaginationParams pagination);

        /// <summary>
        /// Detailed view of a single invoice with all its earning lines.
        /// Throws ForbiddenException if the invoice belongs to a different teacher.
        /// </summary>
        Task<TeacherInvoiceDetailDto> GetInvoiceDetailsAsync(int invoiceId, string teacherId);
    }
}
