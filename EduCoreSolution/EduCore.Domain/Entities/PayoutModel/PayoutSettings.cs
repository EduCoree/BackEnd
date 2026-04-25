using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.PayoutModel
{
    /// <summary>
    /// Global platform-wide payout configuration.
    /// This is a single-row table (Id = 1 always) that admins can update
    /// to change commission rates and tier bonuses without code changes.
    /// </summary>
    public class PayoutSettings : BaseEntity<int>
    {
        // Teacher's share of course revenue (e.g., 0.80 = 80%)
        public decimal TeacherCommissionRate { get; set; } = 0.80m;

        // Tier-based enrollment bonuses (paid enrollments per month)
        // Tier 1: >= Threshold1 AND < Threshold2
        // Tier 2: >= Threshold2 AND < Threshold3
        // Tier 3: >= Threshold3
        // Below Threshold1: no bonus
        public int Tier1Threshold { get; set; } = 10;
        public decimal Tier1Bonus { get; set; } = 500m;

        public int Tier2Threshold { get; set; } = 30;
        public decimal Tier2Bonus { get; set; } = 1500m;

        public int Tier3Threshold { get; set; } = 50;
        public decimal Tier3Bonus { get; set; } = 3000m;

        public string Currency { get; set; } = "EGP";

        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }   // Admin user id

        /// <summary>
        /// Computes the tier bonus for a given number of paid enrollments.
        /// Kept on the entity so both the invoice generation job and the
        /// preview endpoint use identical logic.
        /// </summary>
        public decimal CalculateTierBonus(int paidEnrollmentsCount)
        {
            if (paidEnrollmentsCount >= Tier3Threshold) return Tier3Bonus;
            if (paidEnrollmentsCount >= Tier2Threshold) return Tier2Bonus;
            if (paidEnrollmentsCount >= Tier1Threshold) return Tier1Bonus;
            return 0m;
        }
    }
}
