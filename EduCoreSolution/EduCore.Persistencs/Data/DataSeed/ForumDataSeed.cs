using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.ForumModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.DataSeed
{
   
    public static class ForumDataSeed
    {
        public static async Task SeedAsync(EduCoreDbContext context, UserManager<User> userManager)
        {
            // Skip if forum data already seeded
            if (await context.ForumPosts.AnyAsync())
                return;

            // ── Look up existing users created by DataSeeder ─────────
            var teacher = await userManager.FindByEmailAsync("ahmed.teacher@educore.com");
            var student1 = await userManager.FindByEmailAsync("student1@educore.com");
            var student2 = await userManager.FindByEmailAsync("student2@educore.com");
            var student3 = await userManager.FindByEmailAsync("student3@educore.com");

            if (teacher is null || student1 is null || student2 is null || student3 is null)
            {
                // DataSeeder hasn't run yet — skip Forum seeding
                Console.WriteLine("[ForumDataSeed] Skipped — required users not found. Run DataSeeder first.");
                return;
            }

            // ── Look up an existing published course ─────────────────
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.Title == "Algebra for Beginners");

            if (course is null)
            {
                // Fallback: try any published course
                course = await context.Courses
                    .FirstOrDefaultAsync(c => c.Status == Shared.Enums.CourseStatus.Published);
            }

            if (course is null)
            {
                Console.WriteLine("[ForumDataSeed] Skipped — no published course found. Run DataSeeder first.");
                return;
            }

            // ── Forum Posts ──────────────────────────────────────────
            var post1 = new ForumPost
            {
                CourseId = course.Id,
                StudentId = student1.Id,
                Title = "How to solve quadratic equations?",
                Body = "I'm having trouble understanding the quadratic formula. Can someone explain the steps to solve ax² + bx + c = 0? I keep getting confused with the discriminant part.",
                UpvoteCount = 3,
                IsRemoved = false,
                CreatedAt = DateTime.UtcNow.AddDays(-14)
            };

            var post2 = new ForumPost
            {
                CourseId = course.Id,
                StudentId = student2.Id,
                Title = "Best resources for practicing algebra?",
                Body = "Hey everyone! I'm looking for extra practice problems and resources beyond the course material. Any recommendations for websites or books that helped you?",
                UpvoteCount = 5,
                IsRemoved = false,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            };

            var post3 = new ForumPost
            {
                CourseId = course.Id,
                StudentId = student3.Id,
                Title = "Difference between linear and nonlinear equations",
                Body = "Can someone clarify the key differences between linear and nonlinear equations? I understand linear equations form a straight line, but what makes an equation nonlinear?",
                UpvoteCount = 2,
                IsRemoved = false,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            };

            var post4 = new ForumPost
            {
                CourseId = course.Id,
                StudentId = student1.Id,
                Title = "Tips for the upcoming algebra quiz",
                Body = "The quiz is next week! What topics should we focus on? Does anyone have study tips or know which chapters are most important?",
                UpvoteCount = 8,
                IsRemoved = false,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            };

            var post5 = new ForumPost
            {
                CourseId = course.Id,
                StudentId = student2.Id,
                Title = "I found an error in Lesson 3 notes",
                Body = "In the PDF for lesson 3, the example on page 5 has the wrong sign in step 3. It should be -2x not +2x. Can anyone confirm?",
                UpvoteCount = 1,
                IsRemoved = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            await context.ForumPosts.AddRangeAsync(post1, post2, post3, post4, post5);
            await context.SaveChangesAsync();

            // ── Forum Replies ────────────────────────────────────────
            var replies = new List<ForumReply>
            {
                // Replies to Post 1 — "How to solve quadratic equations?"
                new ForumReply
                {
                    PostId = post1.Id,
                    UserId = teacher.Id,
                    Body = "Great question! The quadratic formula is x = (-b ± √(b²-4ac)) / 2a. The discriminant (b²-4ac) tells you how many solutions exist: positive = 2 real roots, zero = 1 root, negative = no real roots.",
                    UpvoteCount = 4,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-13)
                },
                new ForumReply
                {
                    PostId = post1.Id,
                    UserId = student2.Id,
                    Body = "Thanks for the explanation! I also found it helpful to practice by plugging in simple numbers first before tackling complex problems.",
                    UpvoteCount = 1,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-12)
                },
                new ForumReply
                {
                    PostId = post1.Id,
                    UserId = student3.Id,
                    Body = "I struggled with this too. What helped me was watching the video in Section 2, Lesson 1 twice and taking notes.",
                    UpvoteCount = 2,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-11)
                },

                // Replies to Post 2 — "Best resources for practicing algebra?"
                new ForumReply
                {
                    PostId = post2.Id,
                    UserId = student1.Id,
                    Body = "I've been using Khan Academy alongside this course. Their practice problems are really good for building confidence!",
                    UpvoteCount = 3,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-9)
                },
                new ForumReply
                {
                    PostId = post2.Id,
                    UserId = teacher.Id,
                    Body = "I recommend 'Algebra and Trigonometry' by Stewart. Also, I'll be uploading additional practice sets in the resources section next week.",
                    UpvoteCount = 5,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-8)
                },

                // Replies to Post 3 — "Difference between linear and nonlinear equations"
                new ForumReply
                {
                    PostId = post3.Id,
                    UserId = teacher.Id,
                    Body = "Linear equations have variables with power 1 only (e.g., 2x + 3 = 7). Nonlinear equations have variables with powers other than 1, like x² + 2x = 5 (quadratic) or x³ = 8 (cubic). The graph of a linear equation is always a straight line.",
                    UpvoteCount = 6,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-6)
                },
                new ForumReply
                {
                    PostId = post3.Id,
                    UserId = student1.Id,
                    Body = "Also, nonlinear systems are much harder to solve since you can't just use simple substitution most of the time.",
                    UpvoteCount = 1,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },

                // Replies to Post 4 — "Tips for the upcoming algebra quiz"
                new ForumReply
                {
                    PostId = post4.Id,
                    UserId = student2.Id,
                    Body = "Focus on Chapter 2 (Linear Equations) and Chapter 3 (Quadratics). Those were emphasized the most in class.",
                    UpvoteCount = 3,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new ForumReply
                {
                    PostId = post4.Id,
                    UserId = student3.Id,
                    Body = "Make sure you practice the factoring problems! They tend to come up a lot in quizzes.",
                    UpvoteCount = 2,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new ForumReply
                {
                    PostId = post4.Id,
                    UserId = teacher.Id,
                    Body = "Good discussion! I'll give you a hint — review the summary PDFs at the end of each section. The quiz will cover all sections, but focus especially on problem-solving steps.",
                    UpvoteCount = 7,
                    IsRemoved = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            await context.ForumReplies.AddRangeAsync(replies);
            await context.SaveChangesAsync();

            // ── Post Upvotes ─────────────────────────────────────────
            var upvotes = new List<PostUpvote>
            {
                new PostUpvote { UserId = student2.Id, PostId = post1.Id, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new PostUpvote { UserId = student3.Id, PostId = post1.Id, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new PostUpvote { UserId = teacher.Id,  PostId = post1.Id, CreatedAt = DateTime.UtcNow.AddDays(-11) },

                new PostUpvote { UserId = student1.Id, PostId = post2.Id, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new PostUpvote { UserId = student3.Id, PostId = post2.Id, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new PostUpvote { UserId = teacher.Id,  PostId = post2.Id, CreatedAt = DateTime.UtcNow.AddDays(-8) },

                new PostUpvote { UserId = student1.Id, PostId = post3.Id, CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new PostUpvote { UserId = student2.Id, PostId = post3.Id, CreatedAt = DateTime.UtcNow.AddDays(-5) },

                new PostUpvote { UserId = student2.Id, PostId = post4.Id, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new PostUpvote { UserId = student3.Id, PostId = post4.Id, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new PostUpvote { UserId = teacher.Id,  PostId = post4.Id, CreatedAt = DateTime.UtcNow.AddDays(-1) },

                new PostUpvote { UserId = student3.Id, PostId = post5.Id, CreatedAt = DateTime.UtcNow.AddDays(-1) }
            };

            await context.PostUpvotes.AddRangeAsync(upvotes);
            await context.SaveChangesAsync();

            // ── Post Reports ─────────────────────────────────────────
            var reports = new List<PostReport>
            {
                new PostReport
                {
                    UserId = student3.Id,
                    PostId = post5.Id,
                    Reason = "This might be inaccurate — I checked the PDF and the sign looks correct to me. Could be misleading.",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            await context.PostReports.AddRangeAsync(reports);
            await context.SaveChangesAsync();

            Console.WriteLine("[ForumDataSeed] Successfully seeded 5 posts, 10 replies, 12 upvotes, and 1 report.");
        }
    }
}
