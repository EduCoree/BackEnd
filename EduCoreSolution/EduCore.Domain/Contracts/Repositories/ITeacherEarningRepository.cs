using EduCore.Domain.Entities.PayoutModel;
using EduCore.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    /// <summary>
    /// Repository contract for TeacherEarning entities.
    /// Teacher earnings are created when a paid enrollment is finalized
    /// and later aggregated into monthly invoices.
    /// </summary>
    public interface ITeacherEarningRepository : IGenericRepository<TeacherEarning, int>
    {
        /// <summary>
        /// Guards against creating two earnings for the same payment
        /// (defense in depth — also enforced by unique index at DB level).
        /// </summary>
        Task<bool> ExistsForPaymentAsync(int paymentId);

        /// <summary>
        /// Returns all earnings for a teacher in a period that are still eligible
        /// to be put on an invoice (Status = Available, InvoiceId = null).
        /// Used by the monthly invoice generation job.
        /// </summary>
        Task<IEnumerable<TeacherEarning>> GetAvailableEarningsInPeriodAsync(
            string teacherId, DateTime from, DateTime to);

        /// <summary>
        /// Returns the distinct list of teacher ids that have at least one
        /// Available earning in the given period. Used by the invoice job
        /// to know which teachers need an invoice generated.
        /// </summary>
        Task<IEnumerable<string>> GetDistinctTeachersWithEarningsAsync(
            DateTime from, DateTime to);

        /// <summary>
        /// Paged list of a teacher's earnings (for the teacher dashboard).
        /// Optional date range to restrict to a specific month/quarter.
        /// </summary>
        Task<PagedResult<TeacherEarning>> GetTeacherEarningsPagedAsync(
            string teacherId,
            DateTime? from,
            DateTime? to,
            PaginationParams pagination);

        /// <summary>
        /// All earnings (across all teachers) in a period, for admin reports
        /// and Excel/PDF/CSV export. Filterable by teacher and course.
        /// Returns tracked entities with Course + Teacher includes for report rendering.
        /// </summary>
        Task<IEnumerable<TeacherEarning>> GetEarningsForReportAsync(
            DateTime from,
            DateTime to,
            string? teacherId = null,
            int? courseId = null);

        /// <summary>
        /// Sum of teacher-side net amounts in a period (used for admin dashboard KPIs).
        /// Filterable by teacher for the teacher-side "lifetime earnings" card.
        /// </summary>
        Task<decimal> GetTotalNetAmountAsync(DateTime from, DateTime to, string? teacherId = null);

        /// <summary>
        /// Sum of platform-side fees in a period (admin's real revenue after payouts).
        /// </summary>
        Task<decimal> GetTotalPlatformFeeAsync(DateTime from, DateTime to);
    }
}
