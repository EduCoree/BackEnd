using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.EnrollmentModel
{
    public enum EnrollmentType { Purchase, Free, Gift }
    public enum EnrollmentStatus { Active, Expired, Cancelled }

    public class Enrollment: BaseEntity<int>
    {
        public string StudentId { get; set; }
        public int CourseId { get; set; }
        public EnrollmentType Type { get; set; }
        public DateTime EnrolledAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

        // Navigation
        public User Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public Payment? Payment { get; set; }
    }
}
