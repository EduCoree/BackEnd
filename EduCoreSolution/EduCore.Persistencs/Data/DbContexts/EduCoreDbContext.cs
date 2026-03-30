using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CenterModel;
using EduCore.Domain.Entities.ContentModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.ForumModel;
using EduCore.Domain.Entities.NotificationsModel;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Persistencs.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.DbContexts
{
    public class EduCoreDbContext : IdentityDbContext<User>
    {
        public EduCoreDbContext(DbContextOptions<EduCoreDbContext> options) :base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        #region Dbsets
        // Center
        public DbSet<Center> Centers { get; set; }

        // Auth
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Notifications
        public DbSet<Notification> Notifications { get; set; }

        // Courses
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Lesson> Lessons { get; set; }

        // Content
        public DbSet<VideoLesson> VideoLessons { get; set; }
        public DbSet<PdfLesson> PdfLessons { get; set; }
        public DbSet<LiveSession> LiveSessions { get; set; }

        // Quiz
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<AttemptAnswer> AttemptAnswers { get; set; }

        // Enrollment
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        // Progress
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }

        // Forum
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<ForumReply> ForumReplies { get; set; }
        public DbSet<PostUpvote> PostUpvotes { get; set; }
        public DbSet<PostReport> PostReports { get; set; }



        #endregion
    }
}
