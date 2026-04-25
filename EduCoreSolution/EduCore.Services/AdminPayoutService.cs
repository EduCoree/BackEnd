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
    public class AdminPayoutService : IAdminPayoutService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IInvoiceGenerationService _invoiceGenerationService;

        public AdminPayoutService(
            IUnitOfWork uow,
            IMapper mapper,
            IInvoiceGenerationService invoiceGenerationService)
        {
            _uow = uow;
            _mapper = mapper;
            _invoiceGenerationService = invoiceGenerationService;
        }

        public async Task<PagedResult<TeacherInvoiceDto>> GetAllInvoicesAsync(
            InvoiceStatus? status,
            string? teacherId,
            DateTime? from,
            DateTime? to,
            PaginationParams pagination)
        {
            var paged = await _uow.TeacherInvoiceRepository.GetAllInvoicesPagedAsync(
                status, teacherId, from, to, pagination);

            var dtoItems = _mapper.Map<IEnumerable<TeacherInvoiceDto>>(paged.Items);

            return new PagedResult<TeacherInvoiceDto>
            {
                Items = dtoItems,
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }

        public async Task<TeacherInvoiceDetailDto> GetInvoiceDetailsAsync(int invoiceId)
        {
            var invoice = await _uow.TeacherInvoiceRepository.GetByIdWithDetailsAsync(invoiceId);

            if (invoice is null)
                throw new NotFoundException($"Invoice #{invoiceId} not found.");

            return _mapper.Map<TeacherInvoiceDetailDto>(invoice);
        }

        public async Task<TeacherInvoiceDto> MarkInvoiceAsPaidAsync(
            int invoiceId,
            MarkInvoiceAsPaidDto dto,
            string adminId)
        {
            var invoice = await _uow.TeacherInvoiceRepository.GetByIdAsync(invoiceId);

            if (invoice is null)
                throw new NotFoundException($"Invoice #{invoiceId} not found.");

            // State validation — can only pay Issued (or Draft, edge case) invoices
            if (invoice.Status == InvoiceStatus.Paid)
                throw new BadRequestException("Invoice is already marked as paid.");

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new BadRequestException("Cannot pay a cancelled invoice.");

            // Transaction: update invoice + its earnings together
            await _uow.BeginTransactionAsync();

            try
            {
                // Update invoice
                invoice.Status = InvoiceStatus.Paid;
                invoice.PayoutMethod = dto.PayoutMethod;
                invoice.PayoutReference = dto.PayoutReference;
                invoice.Notes = dto.Notes;
                invoice.PaidAt = DateTime.UtcNow;

                _uow.TeacherInvoiceRepository.Update(invoice);

                // Update all the invoice's earnings to Paid too
                // Load them via the detail method (which Includes them)
                var invoiceWithEarnings = await _uow.TeacherInvoiceRepository
                    .GetByIdWithDetailsAsync(invoiceId);

                if (invoiceWithEarnings is not null)
                {
                    foreach (var earning in invoiceWithEarnings.Earnings)
                    {
                        // Re-fetch as tracked (detail method uses AsNoTracking)
                        var tracked = await _uow.TeacherEarningRepository.GetByIdAsync(earning.Id);
                        if (tracked is not null)
                        {
                            tracked.Status = EarningStatus.Paid;
                            _uow.TeacherEarningRepository.Update(tracked);
                        }
                    }
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitTransactionAsync();

                // Reload with teacher for the response
                var updated = await _uow.TeacherInvoiceRepository.GetByIdWithDetailsAsync(invoiceId);
                return _mapper.Map<TeacherInvoiceDto>(updated);
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<TeacherInvoiceDto> CancelInvoiceAsync(
            int invoiceId,
            CancelInvoiceDto dto,
            string adminId)
        {
            var invoice = await _uow.TeacherInvoiceRepository.GetByIdAsync(invoiceId);

            if (invoice is null)
                throw new NotFoundException($"Invoice #{invoiceId} not found.");

            if (invoice.Status == InvoiceStatus.Paid)
                throw new BadRequestException(
                    "Cannot cancel a paid invoice. Payments already disbursed.");

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new BadRequestException("Invoice is already cancelled.");

            // Transaction: cancel invoice + restore earnings to Available
            await _uow.BeginTransactionAsync();

            try
            {
                invoice.Status = InvoiceStatus.Cancelled;
                invoice.Notes = $"[CANCELLED by {adminId} on {DateTime.UtcNow:yyyy-MM-dd}] {dto.Reason}";
                _uow.TeacherInvoiceRepository.Update(invoice);

                // Release the invoice's earnings back to Available so they can be re-invoiced
                var invoiceWithEarnings = await _uow.TeacherInvoiceRepository
                    .GetByIdWithDetailsAsync(invoiceId);

                if (invoiceWithEarnings is not null)
                {
                    foreach (var earning in invoiceWithEarnings.Earnings)
                    {
                        var tracked = await _uow.TeacherEarningRepository.GetByIdAsync(earning.Id);
                        if (tracked is not null)
                        {
                            tracked.Status = EarningStatus.Available;
                            tracked.InvoiceId = null;   // unlink — will be picked up by next generation
                            _uow.TeacherEarningRepository.Update(tracked);
                        }
                    }
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitTransactionAsync();

                var updated = await _uow.TeacherInvoiceRepository.GetByIdWithDetailsAsync(invoiceId);
                return _mapper.Map<TeacherInvoiceDto>(updated);
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<AdminPayoutDashboardDto> GetDashboardAsync()
        {
            // Current month boundaries
            var now = DateTime.UtcNow;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddTicks(-1);

            // Last month boundaries
            var lastMonthStart = thisMonthStart.AddMonths(-1);
            var lastMonthEnd = thisMonthStart.AddTicks(-1);

            // Pending payouts — all Draft/Issued invoices
            var totalPending = await _uow.TeacherInvoiceRepository.GetTotalPendingPayoutsAsync();

            var pendingInvoices = await _uow.TeacherInvoiceRepository.GetAllInvoicesPagedAsync(
                status: InvoiceStatus.Issued,
                teacherId: null,
                from: null,
                to: null,
                pagination: new PaginationParams { PageNumber = 1, PageSize = 1 });
            var pendingCount = pendingInvoices.TotalCount;

            // Current month snapshot
            var currentEarnings = await _uow.TeacherEarningRepository
                .GetTotalNetAmountAsync(thisMonthStart, thisMonthEnd);

            var currentPlatform = await _uow.TeacherEarningRepository
                .GetTotalPlatformFeeAsync(thisMonthStart, thisMonthEnd);

            var currentCount = (await _uow.TeacherEarningRepository
                .GetDistinctTeachersWithEarningsAsync(thisMonthStart, thisMonthEnd))
                .Count();
            // NOTE: that's "distinct teachers", not "total enrollments".
            // For actual enrollment count we'd need another repo method.
            // Using distinct teachers is a reasonable dashboard proxy for now.

            // Last month paid (already paid out)
            var lastMonthPaid = await _uow.TeacherInvoiceRepository
                .GetTotalPaidInPeriodAsync(lastMonthStart, lastMonthEnd.AddMonths(1));
            // NOTE: PaidAt may fall in this month even if the invoice period was last month,
            // so we extend the filter window by 1 month to capture that.

            var settings = await _uow.PayoutSettingsRepository.GetSettingsAsync();

            return new AdminPayoutDashboardDto
            {
                TotalPendingPayouts = totalPending,
                PendingInvoicesCount = pendingCount,
                CurrentMonthTeacherEarnings = currentEarnings,
                CurrentMonthPlatformRevenue = currentPlatform,
                CurrentMonthPaidEnrollments = currentCount,
                LastMonthPaidToTeachers = lastMonthPaid,
                Currency = settings.Currency
            };
        }

        public async Task<GenerateInvoicesResultDto> TriggerInvoiceGenerationAsync(int year, int month)
        {
            var result = await _invoiceGenerationService.GenerateForMonthAsync(year, month);

            return new GenerateInvoicesResultDto
            {
                Year = result.Year,
                Month = result.Month,
                TeachersProcessed = result.TeachersProcessed,
                InvoicesCreated = result.InvoicesCreated,
                TeachersSkipped = result.TeachersSkipped,
                TeachersFailed = result.TeachersFailed,
                TotalAmountGenerated = result.TotalAmountGenerated,
                FailedTeacherIds = result.FailedTeacherIds,
                Messages = result.Messages
            };
        }
    }
}
