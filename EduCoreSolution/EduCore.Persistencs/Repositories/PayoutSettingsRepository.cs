using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.PayoutModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class PayoutSettingsRepository : IPayoutSettingsRepository
    {
        private readonly EduCoreDbContext _context;

        public PayoutSettingsRepository(EduCoreDbContext context)
        {
            _context = context;
        }

        public async Task<PayoutSettings> GetSettingsAsync()
        {
            // There is always exactly one row (Id = 1) seeded by migration.
            // We still guard against a tampered DB to give a clear error instead of NullReference later.
            var settings = await _context.Set<PayoutSettings>()
                .FirstOrDefaultAsync();

            if (settings is null)
                throw new NotFoundException(
                    "Payout settings row is missing. Check the migration was applied.");

            return settings;
        }

        public async Task UpdateSettingsAsync(PayoutSettings settings, string updatedByAdminId)
        {
            var current = await _context.Set<PayoutSettings>()
                .FirstOrDefaultAsync();

            if (current is null)
                throw new NotFoundException("Payout settings row is missing.");

            // Update in place (don't add a new row — we want Id=1 always)
            current.TeacherCommissionRate = settings.TeacherCommissionRate;
            current.Tier1Threshold = settings.Tier1Threshold;
            current.Tier1Bonus = settings.Tier1Bonus;
            current.Tier2Threshold = settings.Tier2Threshold;
            current.Tier2Bonus = settings.Tier2Bonus;
            current.Tier3Threshold = settings.Tier3Threshold;
            current.Tier3Bonus = settings.Tier3Bonus;
            current.Currency = settings.Currency;
            current.UpdatedAt = DateTime.UtcNow;
            current.UpdatedBy = updatedByAdminId;

            // Caller is expected to call _uow.SaveChangesAsync() after this.
        }
    }
}
