using EduCore.Domain.Entities.CenterModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.ForumModel;
using EduCore.Domain.Entities.NotificationsModel;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Domain.Entities.QuizModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.AuthModel
{
    public enum UserRole { Student, Teacher, Admin }

    public class User : IdentityUser<int>
    {
        public int CenterId { get; set; }
        public string Name { get; set; } = null!;
        public UserRole Role { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        // Navigation — Parent
        public Center Center { get; set; } = null!;

        // Navigation — Children
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Course> TaughtCourses { get; set; } = new List<Course>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
        public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
        public ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();
        public ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
        public ICollection<ForumReply> ForumReplies { get; set; } = new List<ForumReply>();
        public ICollection<PostUpvote> PostUpvotes { get; set; } = new List<PostUpvote>();
        public ICollection<PostReport> PostReports { get; set; } = new List<PostReport>();
    }
}
