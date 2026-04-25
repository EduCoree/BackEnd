using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.EnrollmentModel
{
    public class CashPaymentRequest:BaseEntity<int>
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public CashRequestStatus Status { get; set; } = CashRequestStatus.Pending;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

}
