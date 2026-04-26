using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.PayoutDTOs;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class TeacherPayoutService : ITeacherPayoutService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public TeacherPayoutService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedResult<TeacherEarningDto>> GetMyEarningsAsync(
            string teacherId,
            DateTime? from,
            DateTime? to,
            PaginationParams pagination)
        {
            var pagedEarnings = await _uow.TeacherEarningRepository
                .GetTeacherEarningsPagedAsync(teacherId, from, to, pagination);

            var dtoItems = _mapper.Map<IEnumerable<TeacherEarningDto>>(pagedEarnings.Items);

            return new PagedResult<TeacherEarningDto>
            {
                Items = dtoItems,
                TotalCount = pagedEarnings.TotalCount,
                PageNumber = pagedEarnings.PageNumber,
                PageSize = pagedEarnings.PageSize
            };
        }

        public async Task<CurrentMonthEarningsDto> GetCurrentMonthPreviewAsync(string teacherId)
        {
            // Current month boundaries (UTC)
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

            // All Available earnings in the current month — these would be included
            // in the invoice if the month ended now.
            var earnings = (await _uow.TeacherEarningRepository
                .GetAvailableEarningsInPeriodAsync(teacherId, monthStart, monthEnd))
                .ToList();

            var earningsTotal = earnings.Sum(e => e.NetAmount);
            var paidEnrollmentsCount = earnings.Count;

            // Use the entity's own tier bonus logic
            var settings = await _uow.PayoutSettingsRepository.GetSettingsAsync();
            var projectedBonus = settings.CalculateTierBonus(paidEnrollmentsCount);

            // Figure out next tier (motivational UI)
            int? nextTierThreshold = null;
            decimal? nextTierBonus = null;
            int? toNext = null;

            if (paidEnrollmentsCount < settings.Tier1Threshold)
            {
                nextTierThreshold = settings.Tier1Threshold;
                nextTierBonus = settings.Tier1Bonus;
            }
            else if (paidEnrollmentsCount < settings.Tier2Threshold)
            {
                nextTierThreshold = settings.Tier2Threshold;
                nextTierBonus = settings.Tier2Bonus;
            }
            else if (paidEnrollmentsCount < settings.Tier3Threshold)
            {
                nextTierThreshold = settings.Tier3Threshold;
                nextTierBonus = settings.Tier3Bonus;
            }
            // else: already at max tier — nothing more to unlock

            if (nextTierThreshold.HasValue)
                toNext = nextTierThreshold.Value - paidEnrollmentsCount;

            return new CurrentMonthEarningsDto
            {
                Year = now.Year,
                Month = now.Month,
                PaidEnrollmentsCount = paidEnrollmentsCount,
                EarningsTotal = earningsTotal,
                ProjectedTierBonus = projectedBonus,
                ProjectedTotal = earningsTotal + projectedBonus,
                Currency = settings.Currency,
                NextTierThreshold = nextTierThreshold,
                NextTierBonus = nextTierBonus,
                EnrollmentsToNextTier = toNext
            };
        }

        public async Task<TeacherEarningsSummaryDto> GetEarningsSummaryAsync(string teacherId)
        {
            // Lifetime earnings (not cancelled) — using a wide date range
            var epochStart = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var farFuture = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var totalEarned = await _uow.TeacherEarningRepository
                .GetTotalNetAmountAsync(epochStart, farFuture, teacherId);

            // Sum of Paid + Issued invoices separately
            // We load the teacher's invoices once and aggregate client-side
            // (the repo only exposes paged — for a summary this is fine since a teacher
            //  will typically have <500 invoices over their lifetime)
            var allInvoices = await _uow.TeacherInvoiceRepository.GetTeacherInvoicesPagedAsync(
                teacherId,
                status: null,
                pagination: new PaginationParams { PageNumber = 1, PageSize = int.MaxValue });

            var totalPaid = allInvoices.Items
                .Where(i => i.Status == InvoiceStatus.Paid)
                .Sum(i => i.TotalAmount);

            var totalPending = allInvoices.Items
                .Where(i => i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.Draft)
                .Sum(i => i.TotalAmount);

            var totalPaidEnrollments = allInvoices.Items
                .Where(i => i.Status != InvoiceStatus.Cancelled)
                .Sum(i => i.PaidEnrollmentsCount);

            var settings = await _uow.PayoutSettingsRepository.GetSettingsAsync();

            return new TeacherEarningsSummaryDto
            {
                TotalEarned = totalEarned,
                TotalPaid = totalPaid,
                TotalPending = totalPending,
                TotalInvoicesCount = allInvoices.TotalCount,
                TotalPaidEnrollments = totalPaidEnrollments,
                Currency = settings.Currency
            };
        }

        public async Task<PagedResult<TeacherInvoiceDto>> GetMyInvoicesAsync(
            string teacherId,
            InvoiceStatus? status,
            PaginationParams pagination)
        {
            var pagedInvoices = await _uow.TeacherInvoiceRepository
                .GetTeacherInvoicesPagedAsync(teacherId, status, pagination);

            var dtoItems = _mapper.Map<IEnumerable<TeacherInvoiceDto>>(pagedInvoices.Items);

            return new PagedResult<TeacherInvoiceDto>
            {
                Items = dtoItems,
                TotalCount = pagedInvoices.TotalCount,
                PageNumber = pagedInvoices.PageNumber,
                PageSize = pagedInvoices.PageSize
            };
        }

        public async Task<TeacherInvoiceDetailDto> GetInvoiceDetailsAsync(int invoiceId, string teacherId)
        {
            var invoice = await _uow.TeacherInvoiceRepository.GetByIdWithDetailsAsync(invoiceId);

            if (invoice is null)
                throw new NotFoundException($"Invoice #{invoiceId} not found.");

            // Authorization: teacher can only see their own invoices
            if (invoice.TeacherId != teacherId)
                throw new NotFoundException($"Invoice #{invoiceId} not found.");
            // NOTE: returning NotFound (not Forbidden) to avoid leaking that the invoice exists

            return _mapper.Map<TeacherInvoiceDetailDto>(invoice);
        }
    }
}
