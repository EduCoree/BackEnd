using EduCore.Shared.DTOs.PayoutDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    /// <summary>
    /// Service for reading and updating the global payout configuration
    /// (commission rate + tier thresholds/bonuses).
    /// Read is open to any authenticated user; Update is Admin-only.
    /// </summary>
    public interface IPayoutSettingsService
    {
        /// <summary>
        /// Returns the current active payout settings.
        /// Teachers can read these to understand how their earnings are calculated.
        /// </summary>
        Task<PayoutSettingsDto> GetSettingsAsync();

        /// <summary>
        /// Updates the payout settings. Admin only.
        /// Changes apply to FUTURE earnings only — existing earnings keep
        /// their original commission rate snapshot.
        /// </summary>
        Task<PayoutSettingsDto> UpdateSettingsAsync(UpdatePayoutSettingsDto dto, string adminId);
    }
}
