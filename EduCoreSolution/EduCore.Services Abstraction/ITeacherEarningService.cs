using EduCore.Domain.Entities.PayoutModel;
using EduCore.Domain.Entities.EnrollmentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EduCore.Services_Abstraction
{
    /// <summary>
    /// Responsible for creating and managing TeacherEarning records.
    /// Called by EnrollmentService whenever a paid enrollment is finalized
    /// (Paymob webhook success OR admin recording a cash payment).
    /// </summary>
    public interface ITeacherEarningService
    {
        /// <summary>
        /// Creates a TeacherEarning record for a completed paid enrollment.
        ///
        /// IMPORTANT:
        ///   - Does NOT call SaveChangesAsync. The caller (EnrollmentService)
        ///     is responsible for the transaction and saving.
        ///   - Does NOT accept a commission rate — it reads it from PayoutSettings
        ///     at the moment of the call (snapshot).
        ///   - Idempotent: if an earning already exists for this payment, returns
        ///     the existing one without creating a duplicate.
        /// </summary>
        /// <param name="payment">The completed payment (must have Status = Completed)</param>
        /// <param name="enrollment">The enrollment created from this payment</param>
        /// <returns>The newly created (or pre-existing) TeacherEarning</returns>
        Task<TeacherEarning> CreateEarningForPaymentAsync(
            Payment payment,
            Enrollment enrollment);
    }
}
