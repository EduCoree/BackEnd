using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.PayoutModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class TeacherEarningService : ITeacherEarningService
    {
        private readonly IUnitOfWork _uow;

        public TeacherEarningService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TeacherEarning> CreateEarningForPaymentAsync(
            Payment payment,
            Enrollment enrollment)
        {
            // ── Guard 1: Payment must be Completed ──
            if (payment.Status != PaymentStatus.Completed)
                throw new BadRequestException(
                    $"Cannot create earning for payment #{payment.Id}: status is {payment.Status}, expected Completed.");

            // ── Guard 2: Prevent duplicate earning for the same payment ──
            // (Idempotency: webhook might fire twice, admin might click twice, etc.)
            if (await _uow.TeacherEarningRepository.ExistsForPaymentAsync(payment.Id))
            {
                // Load and return the existing one — no duplicate created
                var existing = (await _uow.TeacherEarningRepository.GetAllAsync())
                    .FirstOrDefault(e => e.PaymentId == payment.Id);

                if (existing is not null)
                    return existing;
            }

            // ── Load the course to get TeacherId and confirm it exists ──
            var course = await _uow.CourseRepository.GetByIdAsync(enrollment.CourseId);
            if (course is null)
                throw new NotFoundException($"Course #{enrollment.CourseId} not found.");

            if (string.IsNullOrEmpty(course.TeacherId))
                throw new BadRequestException(
                    $"Course #{course.Id} has no TeacherId — cannot create earning.");

            // ── Load current payout settings (commission rate snapshot) ──
            var settings = await _uow.PayoutSettingsRepository.GetSettingsAsync();
            var rate = settings.TeacherCommissionRate;

            // ── Calculate the split ──
            // Round to 2 decimal places to match currency precision
            // PlatformFee = Amount - NetAmount (subtract to avoid rounding drift)
            var netAmount = Math.Round(payment.Amount * rate, 2, MidpointRounding.AwayFromZero);
            var platformFee = payment.Amount - netAmount;

            // ── Build the earning record ──
            var earning = new TeacherEarning
            {
                TeacherId = course.TeacherId,
                CourseId = course.Id,
                PaymentId = payment.Id,
                EnrollmentId = enrollment.Id,
                GrossAmount = payment.Amount,
                CommissionRate = rate,
                NetAmount = netAmount,
                PlatformFee = platformFee,
                Currency = payment.Currency,
                EarnedAt = payment.PaidAt ?? DateTime.UtcNow,
                Status = EarningStatus.Available,
                InvoiceId = null
            };

            await _uow.TeacherEarningRepository.AddAsync(earning);

            // NOTE: Intentionally NOT calling SaveChangesAsync here.
            // The caller (EnrollmentService) is running inside a DB transaction
            // and will save everything atomically.

            return earning;
        }
    }
}
