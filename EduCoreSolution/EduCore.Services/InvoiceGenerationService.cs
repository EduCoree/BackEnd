using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.PayoutModel;
using EduCore.Shared.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public class InvoiceGenerationService : IInvoiceGenerationService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<InvoiceGenerationService> _logger;

        public InvoiceGenerationService(
            IUnitOfWork uow,
            ILogger<InvoiceGenerationService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public Task<InvoiceGenerationResult> GenerateForPreviousMonthAsync()
        {
            // "Previous month" relative to today. Called on day 1 of each month,
            // so this computes the month that just ended.
            var today = DateTime.UtcNow;
            var firstOfThisMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastMonth = firstOfThisMonth.AddMonths(-1);

            return GenerateForMonthAsync(lastMonth.Year, lastMonth.Month);
        }

        public async Task<InvoiceGenerationResult> GenerateForMonthAsync(int year, int month)
        {
            // ── Validate & compute period boundaries ──
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), "Month must be 1..12");

            var periodStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            // Last tick of the month (e.g., 2026-03-31 23:59:59.9999999)
            var periodEnd = periodStart.AddMonths(1).AddTicks(-1);

            var result = new InvoiceGenerationResult
            {
                Year = year,
                Month = month,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd
            };

            _logger.LogInformation(
                "Starting invoice generation for {Year}-{Month:D2} (period {Start:yyyy-MM-dd} to {End:yyyy-MM-dd})",
                year, month, periodStart, periodEnd);

            // ── Load payout settings once (used for all teachers) ──
            var settings = await _uow.PayoutSettingsRepository.GetSettingsAsync();

            // ── Find all teachers with earnings in the period ──
            var teacherIds = (await _uow.TeacherEarningRepository
                .GetDistinctTeachersWithEarningsAsync(periodStart, periodEnd))
                .ToList();

            _logger.LogInformation(
                "Found {Count} teachers with earnings in the period.", teacherIds.Count);

            // ── Process each teacher independently ──
            // Important: per-teacher transaction so one failure doesn't block others.
            foreach (var teacherId in teacherIds)
            {
                result.TeachersProcessed++;
                try
                {
                    var created = await GenerateInvoiceForTeacherAsync(
                        teacherId, periodStart, periodEnd, settings, result);

                    if (created is not null)
                    {
                        result.InvoicesCreated++;
                        result.TotalAmountGenerated += created.TotalAmount;
                    }
                    else
                    {
                        result.TeachersSkipped++;
                    }
                }
                catch (Exception ex)
                {
                    result.TeachersFailed++;
                    result.FailedTeacherIds.Add(teacherId);
                    result.Messages.Add($"Teacher {teacherId}: {ex.Message}");

                    _logger.LogError(ex,
                        "Failed generating invoice for teacher {TeacherId} in period {Year}-{Month:D2}",
                        teacherId, year, month);

                    // Continue with the next teacher
                }
            }

            _logger.LogInformation(
                "Invoice generation complete. Created: {Created}, Skipped: {Skipped}, Failed: {Failed}, Total: {Total} EGP",
                result.InvoicesCreated, result.TeachersSkipped, result.TeachersFailed, result.TotalAmountGenerated);

            return result;
        }

        /// <summary>
        /// Generates a single invoice for one teacher within a transaction.
        /// Returns null if the teacher was skipped (already had an invoice for this period).
        /// Returns the created invoice otherwise.
        /// </summary>
        private async Task<TeacherInvoice?> GenerateInvoiceForTeacherAsync(
            string teacherId,
            DateTime periodStart,
            DateTime periodEnd,
            PayoutSettings settings,
            InvoiceGenerationResult result)
        {
            // ── Idempotency check (also protected by DB unique index) ──
            if (await _uow.TeacherInvoiceRepository.ExistsForPeriodAsync(teacherId, periodStart, periodEnd))
            {
                _logger.LogInformation(
                    "Teacher {TeacherId} already has invoice for period — skipping.", teacherId);
                result.Messages.Add($"Teacher {teacherId}: already has invoice for this period.");
                return null;
            }

            // ── Transaction wraps: invoice creation + earnings update ──
            await _uow.BeginTransactionAsync();

            try
            {
                // Load earnings that are Available and unassigned to any invoice
                var earnings = (await _uow.TeacherEarningRepository
                    .GetAvailableEarningsInPeriodAsync(teacherId, periodStart, periodEnd))
                    .ToList();

                if (earnings.Count == 0)
                {
                    // Edge case: teacher appeared in the distinct list but all their
                    // earnings got picked up by a concurrent run. Skip cleanly.
                    await _uow.RollbackTransactionAsync();
                    return null;
                }

                // Sum net amounts (what the teacher earned from courses)
                var earningsTotal = earnings.Sum(e => e.NetAmount);

                // Paid enrollment count = number of earnings (one earning == one paid enrollment)
                var paidEnrollmentsCount = earnings.Count;

                // Compute tier bonus using the entity's own logic
                var tierBonus = settings.CalculateTierBonus(paidEnrollmentsCount);

                var totalAmount = earningsTotal + tierBonus;

                // Generate the next invoice number for this month
                var invoiceNumber = await _uow.TeacherInvoiceRepository
                    .GenerateNextInvoiceNumberAsync(periodStart.Year, periodStart.Month);

                var invoice = new TeacherInvoice
                {
                    InvoiceNumber = invoiceNumber,
                    TeacherId = teacherId,
                    PeriodStart = periodStart,
                    PeriodEnd = periodEnd,
                    PaidEnrollmentsCount = paidEnrollmentsCount,
                    EarningsTotal = earningsTotal,
                    TierBonus = tierBonus,
                    TotalAmount = totalAmount,
                    Currency = settings.Currency,
                    // Issued directly: simpler flow, admin can still cancel if needed
                    Status = InvoiceStatus.Issued,
                    CreatedAt = DateTime.UtcNow,
                    IssuedAt = DateTime.UtcNow
                };

                await _uow.TeacherInvoiceRepository.AddAsync(invoice);
                await _uow.SaveChangesAsync(); // flush so invoice.Id is generated

                // Link all earnings to this invoice
                foreach (var earning in earnings)
                {
                    earning.InvoiceId = invoice.Id;
                    earning.Status = EarningStatus.Invoiced;
                    _uow.TeacherEarningRepository.Update(earning);
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitTransactionAsync();

                _logger.LogInformation(
                    "Invoice {InvoiceNumber} created for teacher {TeacherId}: {Earnings} earnings + {Bonus} bonus = {Total} {Currency}",
                    invoiceNumber, teacherId, earningsTotal, tierBonus, totalAmount, settings.Currency);

                return invoice;
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
