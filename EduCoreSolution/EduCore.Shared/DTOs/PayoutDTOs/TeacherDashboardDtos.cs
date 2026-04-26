using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.PayoutDTOs
{
    /// <summary>
    /// "How much have I earned so far this month" preview — teacher dashboard.
    /// Shows a real-time estimate BEFORE the invoice is generated at month-end.
    /// </summary>
    public class CurrentMonthEarningsDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public int PaidEnrollmentsCount { get; set; }   // Number of paid enrollments this month
        public decimal EarningsTotal { get; set; }      // Sum of NetAmount so far
        public decimal ProjectedTierBonus { get; set; } // What bonus they'd get if the month ended now
        public decimal ProjectedTotal { get; set; }     // EarningsTotal + ProjectedTierBonus
        public string Currency { get; set; } = "EGP";

        // Progress toward next tier (for UI motivation)
        public int? NextTierThreshold { get; set; }     // e.g., 30 if they're at 25
        public int? EnrollmentsToNextTier { get; set; } // e.g., 5 (= 30 - 25)
        public decimal? NextTierBonus { get; set; }     // e.g., 1500
    }

    /// <summary>
    /// Lifetime earnings summary for the teacher dashboard.
    /// </summary>
    public class TeacherEarningsSummaryDto
    {
        public decimal TotalEarned { get; set; }        // All-time NetAmount sum
        public decimal TotalPaid { get; set; }          // Sum of Paid invoices
        public decimal TotalPending { get; set; }       // Sum of Issued invoices
        public int TotalInvoicesCount { get; set; }
        public int TotalPaidEnrollments { get; set; }   // All-time paid students
        public string Currency { get; set; } = "EGP";
    }
}
