using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.PayoutModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Common;
using EduCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class TeacherEarningRepository : GenericRepository<TeacherEarning, int>, ITeacherEarningRepository
    {
        public TeacherEarningRepository(EduCoreDbContext context) : base(context) { }

        public async Task<bool> ExistsForPaymentAsync(int paymentId)
        {
            return await _EduCoreDbContext.Set<TeacherEarning>()
                .AnyAsync(e => e.PaymentId == paymentId);
        }

        public async Task<IEnumerable<TeacherEarning>> GetAvailableEarningsInPeriodAsync(
            string teacherId, DateTime from, DateTime to)
        {
            // Returns tracked entities — the invoice generation job needs
            // to update them (set InvoiceId + change status to Invoiced).
            return await _EduCoreDbContext.Set<TeacherEarning>()
                .Where(e => e.TeacherId == teacherId
                         && e.Status == EarningStatus.Available
                         && e.InvoiceId == null
                         && e.EarnedAt >= from
                         && e.EarnedAt <= to)
                .OrderBy(e => e.EarnedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctTeachersWithEarningsAsync(
            DateTime from, DateTime to)
        {
            return await _EduCoreDbContext.Set<TeacherEarning>()
                .AsNoTracking()
                .Where(e => e.Status == EarningStatus.Available
                         && e.InvoiceId == null
                         && e.EarnedAt >= from
                         && e.EarnedAt <= to)
                .Select(e => e.TeacherId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<PagedResult<TeacherEarning>> GetTeacherEarningsPagedAsync(
            string teacherId,
            DateTime? from,
            DateTime? to,
            PaginationParams pagination)
        {
            var query = _EduCoreDbContext.Set<TeacherEarning>()
                .AsNoTracking()
                .Include(e => e.Course)
                .Where(e => e.TeacherId == teacherId);

            if (from.HasValue)
                query = query.Where(e => e.EarnedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(e => e.EarnedAt <= to.Value);

            // Count BEFORE paging (total of filtered set)
            var total = await query.CountAsync();

            // Paging happens at DB level via Skip/Take translated to SQL OFFSET/FETCH
            var items = await query
                .OrderByDescending(e => e.EarnedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResult<TeacherEarning>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<IEnumerable<TeacherEarning>> GetEarningsForReportAsync(
            DateTime from,
            DateTime to,
            string? teacherId = null,
            int? courseId = null)
        {
            var query = _EduCoreDbContext.Set<TeacherEarning>()
                .AsNoTracking()
                .Include(e => e.Course)
                .Include(e => e.Teacher)
                .Include(e => e.Payment)
                .Where(e => e.EarnedAt >= from && e.EarnedAt <= to);

            if (!string.IsNullOrEmpty(teacherId))
                query = query.Where(e => e.TeacherId == teacherId);

            if (courseId.HasValue)
                query = query.Where(e => e.CourseId == courseId.Value);

            return await query
                .OrderByDescending(e => e.EarnedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalNetAmountAsync(
            DateTime from, DateTime to, string? teacherId = null)
        {
            var query = _EduCoreDbContext.Set<TeacherEarning>()
                .AsNoTracking()
                .Where(e => e.EarnedAt >= from && e.EarnedAt <= to
                         && e.Status != EarningStatus.Cancelled);

            if (!string.IsNullOrEmpty(teacherId))
                query = query.Where(e => e.TeacherId == teacherId);

            // SumAsync returns 0 on empty (no null handling needed)
            return await query.SumAsync(e => e.NetAmount);
        }

        public async Task<decimal> GetTotalPlatformFeeAsync(DateTime from, DateTime to)
        {
            return await _EduCoreDbContext.Set<TeacherEarning>()
                .AsNoTracking()
                .Where(e => e.EarnedAt >= from && e.EarnedAt <= to
                         && e.Status != EarningStatus.Cancelled)
                .SumAsync(e => e.PlatformFee);
        }
    }
}
