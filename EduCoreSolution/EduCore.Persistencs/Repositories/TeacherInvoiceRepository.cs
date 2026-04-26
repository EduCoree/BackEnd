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
    public class TeacherInvoiceRepository : GenericRepository<TeacherInvoice, int>, ITeacherInvoiceRepository
    {
        public TeacherInvoiceRepository(EduCoreDbContext context) : base(context) { }

        public async Task<TeacherInvoice?> GetByIdWithDetailsAsync(int invoiceId)
        {
            return await _EduCoreDbContext.Set<TeacherInvoice>()
                .AsNoTracking()
                .Include(i => i.Teacher)
                .Include(i => i.Earnings)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
        }

        public async Task<PagedResult<TeacherInvoice>> GetTeacherInvoicesPagedAsync(
            string teacherId,
            InvoiceStatus? status,
            PaginationParams pagination)
        {
            var query = _EduCoreDbContext.Set<TeacherInvoice>()
                .AsNoTracking()
                .Where(i => i.TeacherId == teacherId);

            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.PeriodStart)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResult<TeacherInvoice>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResult<TeacherInvoice>> GetAllInvoicesPagedAsync(
            InvoiceStatus? status,
            string? teacherId,
            DateTime? from,
            DateTime? to,
            PaginationParams pagination)
        {
            var query = _EduCoreDbContext.Set<TeacherInvoice>()
                .AsNoTracking()
                .Include(i => i.Teacher)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);

            if (!string.IsNullOrEmpty(teacherId))
                query = query.Where(i => i.TeacherId == teacherId);

            if (from.HasValue)
                query = query.Where(i => i.PeriodStart >= from.Value);

            if (to.HasValue)
                query = query.Where(i => i.PeriodEnd <= to.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResult<TeacherInvoice>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<bool> ExistsForPeriodAsync(
            string teacherId, DateTime periodStart, DateTime periodEnd)
        {
            return await _EduCoreDbContext.Set<TeacherInvoice>()
                .AnyAsync(i => i.TeacherId == teacherId
                            && i.PeriodStart == periodStart
                            && i.PeriodEnd == periodEnd);
        }

        public async Task<string> GenerateNextInvoiceNumberAsync(int year, int month)
        {
            // Prefix like "INV-2026-04-"
            var prefix = $"INV-{year:D4}-{month:D2}-";

            // Find the highest sequence number used for this year/month
            var lastInvoice = await _EduCoreDbContext.Set<TeacherInvoice>()
                .AsNoTracking()
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .Select(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            var nextSequence = 1;
            if (!string.IsNullOrEmpty(lastInvoice))
            {
                // Extract the trailing number (3 digits after the last '-')
                var parts = lastInvoice.Split('-');
                if (parts.Length == 4 && int.TryParse(parts[3], out var lastSeq))
                {
                    nextSequence = lastSeq + 1;
                }
            }

            return $"{prefix}{nextSequence:D3}";
        }

        public async Task<decimal> GetTotalPendingPayoutsAsync()
        {
            // Draft + Issued = money we still owe teachers
            return await _EduCoreDbContext.Set<TeacherInvoice>()
                .AsNoTracking()
                .Where(i => i.Status == InvoiceStatus.Draft
                         || i.Status == InvoiceStatus.Issued)
                .SumAsync(i => i.TotalAmount);
        }

        public async Task<decimal> GetTotalPaidInPeriodAsync(DateTime from, DateTime to)
        {
            return await _EduCoreDbContext.Set<TeacherInvoice>()
                .AsNoTracking()
                .Where(i => i.Status == InvoiceStatus.Paid
                         && i.PaidAt.HasValue
                         && i.PaidAt.Value >= from
                         && i.PaidAt.Value <= to)
                .SumAsync(i => i.TotalAmount);
        }
    }
}
