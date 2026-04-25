using EduCore.Domain.Entities.PayoutModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    /// <summary>
    /// Repository contract for PayoutSettings.
    /// This is intentionally NOT a generic repository — PayoutSettings is a
    /// single-row configuration table (Id always = 1).
    /// </summary>
    public interface IPayoutSettingsRepository
    {
        /// <summary>
        /// Returns the current active payout configuration.
        /// Always returns a non-null result — if the row is missing (which shouldn't
        /// happen since it's seeded by migration), throws an exception.
        /// </summary>
        Task<PayoutSettings> GetSettingsAsync();

        /// <summary>
        /// Updates the current payout configuration. Tracks who made the change.
        /// Call SaveChangesAsync on the UnitOfWork after this.
        /// </summary>
        Task UpdateSettingsAsync(PayoutSettings settings, string updatedByAdminId);
    }
}
