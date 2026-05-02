using EduCore.Domain.Entities.ProgressModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduCore.Persistencs.Data.DataSeed.Seeders
{
    public static class ProgressSeeder
    {
        public static async Task SeedLessonProgressAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.LessonProgresses.AnyAsync()) return;

            // For every active enrollment, mark the first half of lessons as completed
            var enrollments = await context.Enrollments
                .Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Sections)
                        .ThenInclude(s => s.Lessons)
                .ToListAsync();

            var rows = new List<LessonProgress>();
            foreach (var en in enrollments)
            {
                var lessons = en.Course.Sections
                    .OrderBy(s => s.SortOrder)
                    .SelectMany(s => s.Lessons.OrderBy(l => l.SortOrder))
                    .ToList();
                if (!lessons.Any()) continue;

                var completedCount = en.Status == EnrollmentStatus.Completed
                    ? lessons.Count
                    : Math.Max(1, lessons.Count / 2);

                for (int i = 0; i < lessons.Count; i++)
                {
                    var l = lessons[i];
                    var done = i < completedCount;
                    rows.Add(new LessonProgress
                    {
                        StudentId = en.StudentId,
                        LessonId = l.Id,
                        IsCompleted = done,
                        CompletedAt = done ? DateTime.UtcNow.AddDays(-1) : null,
                        LastPositionSecs = (l.DurationSeconds.HasValue && !done) ? l.DurationSeconds / 3 : null
                    });
                }
            }

            if (rows.Any())
            {
                context.LessonProgresses.AddRange(rows);
                await context.SaveChangesAsync();
                logger.LogInformation("LessonProgress rows seeded ({Count}).", rows.Count);
            }
        }

        public static async Task SeedAttendanceAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.AttendanceRecords.AnyAsync()) return;

            var liveSessions = await context.LiveSessions.ToListAsync();
            if (!liveSessions.Any()) return;

            // Find all students enrolled in courses that have any of these live sessions
            var rows = new List<AttendanceRecord>();
            foreach (var session in liveSessions)
            {
                var studentIds = await context.Enrollments
                    .Where(e => e.CourseId == session.CourseId)
                    .Select(e => e.StudentId)
                    .ToListAsync();

                int idx = 0;
                foreach (var sid in studentIds)
                {
                    rows.Add(new AttendanceRecord
                    {
                        StudentId = sid,
                        LiveSessionId = session.Id,
                        JoinedAt = session.ScheduledAt.AddMinutes(idx % 3 == 0 ? 5 : 0),
                        Status = (idx % 4) switch
                        {
                            0 => AttendanceStatus.Late,
                            1 => AttendanceStatus.Absent,
                            _ => AttendanceStatus.Attended
                        }
                    });
                    idx++;
                }
            }

            if (rows.Any())
            {
                context.AttendanceRecords.AddRange(rows);
                await context.SaveChangesAsync();
                logger.LogInformation("AttendanceRecords seeded ({Count}).", rows.Count);
            }
        }

        public static async Task SeedCertificatesAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.Certificates.AnyAsync()) return;

            // One certificate per Completed enrollment
            var completed = await context.Enrollments
                .Where(e => e.Status == EnrollmentStatus.Completed)
                .ToListAsync();

            var rows = completed.Select(e => new Certificate
            {
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                IssuedAt = DateTime.UtcNow.AddDays(-1),
                CertificateUrl = $"https://cdn.educore.com/certificates/{e.StudentId}-{e.CourseId}.pdf"
            }).ToList();

            if (rows.Any())
            {
                context.Certificates.AddRange(rows);
                await context.SaveChangesAsync();
                logger.LogInformation("Certificates seeded ({Count}).", rows.Count);
            }
        }

        public static async Task SeedCourseReviewsAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.CourseReviews.AnyAsync()) return;

            var enrollments = await context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed)
                .ToListAsync();

            // Mix of English and Arabic reviews
            var samples = new (int rating, string comment)[]
            {
                (5, "Excellent course! Highly recommend."),
                (4, "Great content, well explained."),
                (5, "كورس ممتاز جداً، استفدت منه كتير. الشرح واضح والمدرس متمكن."),
                (3, "Decent material, could use more examples."),
                (5, "والله من أحسن الكورسات اللي اشتركت فيها. شكراً جزيلاً للمدرس."),
                (4, " الشرح عااااااالي اوييي")
            };

            var reviews = new List<CourseReview>();
            int i = 0;
            foreach (var en in enrollments.Take(samples.Length))
            {
                var (rating, comment) = samples[i++];
                reviews.Add(new CourseReview
                {
                    StudentId = en.StudentId,
                    CourseId = en.CourseId,
                    Rating = (byte)rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow.AddDays(-i)
                });
            }

            if (reviews.Any())
            {
                context.CourseReviews.AddRange(reviews);
                await context.SaveChangesAsync();
                logger.LogInformation("CourseReviews seeded ({Count}).", reviews.Count);
            }
        }
    }
}