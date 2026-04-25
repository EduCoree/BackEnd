using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.PayoutDTOs
{
    /// <summary>
    /// Current payout rules — shown to admin and to teachers
    /// (teachers see it as info about how earnings work).
    /// </summary>
    public class PayoutSettingsDto
    {
        public decimal TeacherCommissionRate { get; set; }   // 0.80

        public int Tier1Threshold { get; set; }
        public decimal Tier1Bonus { get; set; }

        public int Tier2Threshold { get; set; }
        public decimal Tier2Bonus { get; set; }

        public int Tier3Threshold { get; set; }
        public decimal Tier3Bonus { get; set; }

        public string Currency { get; set; } = "EGP";

        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    /// <summary>
    /// Admin's request to update the payout rules.
    /// Validations keep the values in sensible ranges.
    /// </summary>
    public class UpdatePayoutSettingsDto
    {
        [Range(0.0, 1.0, ErrorMessage = "Commission rate must be between 0.0 and 1.0")]
        public decimal TeacherCommissionRate { get; set; }

        [Range(1, 10000)]
        public int Tier1Threshold { get; set; }
        [Range(0, 1000000)]
        public decimal Tier1Bonus { get; set; }

        [Range(1, 10000)]
        public int Tier2Threshold { get; set; }
        [Range(0, 1000000)]
        public decimal Tier2Bonus { get; set; }

        [Range(1, 10000)]
        public int Tier3Threshold { get; set; }
        [Range(0, 1000000)]
        public decimal Tier3Bonus { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "EGP";
    }
}
