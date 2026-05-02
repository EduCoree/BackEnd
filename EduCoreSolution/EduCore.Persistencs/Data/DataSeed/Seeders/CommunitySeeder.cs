using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.ChatModel;
using EduCore.Domain.Entities.ForumModel;
using EduCore.Domain.Entities.NotificationsModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduCore.Persistencs.Data.DataSeed.Seeders
{
    public static class CommunitySeeder
    {
        public static async Task SeedForumAsync(
            EduCoreDbContext context, UserManager<User> userManager, ILogger logger)
        {
            if (await context.ForumPosts.AnyAsync()) return;

            var algebraLesson = await context.Lessons.FirstOrDefaultAsync(l => l.Title == "What is Algebra?");
            var biologyLesson = await context.Lessons.FirstOrDefaultAsync(l => l.Title == "Cell Membrane");
            var nahwLesson = await context.Lessons.FirstOrDefaultAsync(l => l.Title == "المبتدأ والخبر");

            var s1 = await userManager.FindByEmailAsync("ali.mahmoud@educore.com");
            var s2 = await userManager.FindByEmailAsync("nour.hassan@educore.com");
            var s3 = await userManager.FindByEmailAsync("youssef.ibrahim@educore.com");
            var t1 = await userManager.FindByEmailAsync("ahmed.tawfik@educore.com");
            var t3 = await userManager.FindByEmailAsync("menna.abulela@educore.com");
            var admin = await userManager.FindByEmailAsync("admin@educore.com");
            if (s1 == null || s2 == null || t1 == null) return;

            var posts = new List<ForumPost>();

            // English forum post on English lesson
            if (algebraLesson != null)
            {
                posts.Add(new ForumPost
                {
                    LessonId = algebraLesson.Id,
                    StudentId = s1.Id,
                    Title = "Help with algebraic variables",
                    Body = "I'm having trouble understanding how variables work. Can someone give an example?",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpvoteCount = 2,
                    Replies = new List<ForumReply>
                    {
                        new ForumReply { UserId = s2.Id, Body = "Variables are like empty boxes — you can put different numbers in them!", CreatedAt = DateTime.UtcNow.AddDays(-1) },
                        new ForumReply { UserId = t1.Id, Body = "Good explanation! Think of x as a placeholder for a value we want to find.", CreatedAt = DateTime.UtcNow.AddHours(-12) }
                    },
                    Upvotes = new List<PostUpvote>
                    {
                        new PostUpvote { UserId = s2.Id, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                        new PostUpvote { UserId = t1.Id, CreatedAt = DateTime.UtcNow.AddHours(-12) }
                    }
                });

                posts.Add(new ForumPost
                {
                    LessonId = algebraLesson.Id,
                    StudentId = s2.Id,
                    Title = "Great lesson!",
                    Body = "The explanation was very clear, thank you.",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpvoteCount = 0
                });
            }

            // English forum post (with Arabic reply mixed in — realistic)
            if (biologyLesson != null)
            {
                posts.Add(new ForumPost
                {
                    LessonId = biologyLesson.Id,
                    StudentId = s1.Id,
                    Title = "Question about cell membranes",
                    Body = "What does semi-permeable actually mean in this context?",
                    CreatedAt = DateTime.UtcNow.AddHours(-5),
                    UpvoteCount = 1,
                    Replies = new List<ForumReply>
                    {
                        new ForumReply { UserId = s3.Id, Body = "يعني إنه يسمح بمرور بعض المواد ويمنع غيرها — انتقائي.", CreatedAt = DateTime.UtcNow.AddHours(-3) }
                    },
                    Upvotes = new List<PostUpvote>
                    {
                        new PostUpvote { UserId = s2.Id, CreatedAt = DateTime.UtcNow.AddHours(-2) }
                    },
                    Reports = admin != null ? new List<PostReport>
                    {
                        new PostReport { UserId = admin.Id, Reason = "Test report — duplicate question", CreatedAt = DateTime.UtcNow.AddHours(-1) }
                    } : new List<PostReport>()
                });
            }

            // Arabic forum post on Arabic lesson
            if (nahwLesson != null && t3 != null && s3 != null)
            {
                posts.Add(new ForumPost
                {
                    LessonId = nahwLesson.Id,
                    StudentId = s3.Id,
                    Title = "سؤال عن الفرق بين المبتدأ والفاعل",
                    Body = "ممكن حد يوضح لي الفرق بين المبتدأ والفاعل؟ بيتلخبطوا عليّ كتير.",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpvoteCount = 3,
                    Replies = new List<ForumReply>
                    {
                        new ForumReply { UserId = s2.Id, Body = "المبتدأ بييجي في الجملة الاسمية، والفاعل في الجملة الفعلية.", CreatedAt = DateTime.UtcNow.AddHours(-20) },
                        new ForumReply { UserId = t3.Id, Body = "إجابة ممتازة. باختصار: لو الجملة بدأت باسم → مبتدأ، لو بدأت بفعل → اللي بعد الفعل فاعل.", CreatedAt = DateTime.UtcNow.AddHours(-18) }
                    },
                    Upvotes = new List<PostUpvote>
                    {
                        new PostUpvote { UserId = s1.Id, CreatedAt = DateTime.UtcNow.AddHours(-19) },
                        new PostUpvote { UserId = s2.Id, CreatedAt = DateTime.UtcNow.AddHours(-18) },
                        new PostUpvote { UserId = t3.Id, CreatedAt = DateTime.UtcNow.AddHours(-17) }
                    }
                });
            }

            if (posts.Any())
            {
                context.ForumPosts.AddRange(posts);
                await context.SaveChangesAsync();
                logger.LogInformation("Forum seeded — Posts: {P}, Replies: {R}, Upvotes: {U}",
                    posts.Count, posts.Sum(p => p.Replies.Count), posts.Sum(p => p.Upvotes.Count));
            }
        }

        public static async Task SeedNotificationsAsync(
            EduCoreDbContext context, UserManager<User> userManager, ILogger logger)
        {
            if (await context.Notifications.AnyAsync()) return;

            var s1 = await userManager.FindByEmailAsync("ali.mahmoud@educore.com");
            var s2 = await userManager.FindByEmailAsync("nour.hassan@educore.com");
            var s3 = await userManager.FindByEmailAsync("youssef.ibrahim@educore.com");
            var t1 = await userManager.FindByEmailAsync("ahmed.tawfik@educore.com");
            var t3 = await userManager.FindByEmailAsync("menna.abulela@educore.com");
            if (s1 == null || s2 == null || t1 == null) return;

            var rows = new List<Notification>
            {
                // English notifications
                new Notification { UserId = s1.Id, Type = "Enrollment",  Title = "Welcome to Algebra for Beginners!", Message = "You've successfully enrolled.",            CreatedAt = DateTime.UtcNow.AddDays(-25), IsRead = true },
                new Notification { UserId = s1.Id, Type = "QuizResult",  Title = "Quiz passed!",                      Message = "You scored 25/25 on Algebra Basics Quiz.",  CreatedAt = DateTime.UtcNow.AddDays(-3),  IsRead = false },
                new Notification { UserId = s2.Id, Type = "ForumReply",  Title = "New reply on your post",            Message = "Ahmed Tawfik replied to your question.",     CreatedAt = DateTime.UtcNow.AddHours(-12),IsRead = false },
                new Notification { UserId = t1.Id, Type = "Enrollment",  Title = "New student enrolled",              Message = "A student joined Algebra for Beginners.",    CreatedAt = DateTime.UtcNow.AddDays(-25), IsRead = true },

                // Arabic notifications
                new Notification { UserId = s3.Id, Type = "Enrollment",  Title = "أهلاً بك في كورس النحو العربي",     Message = "تم تسجيلك بنجاح في كورس \"أساسيات النحو العربي\".", CreatedAt = DateTime.UtcNow.AddDays(-8),  IsRead = true },
                new Notification { UserId = s3.Id, Type = "QuizResult",  Title = "نتيجة الاختبار",                    Message = "حصلت على 20/25 في اختبار النحو العربي. مبروك!",     CreatedAt = DateTime.UtcNow.AddDays(-2),  IsRead = false },
                new Notification { UserId = t3.Id, Type = "LiveSession", Title = "تذكير بحصة مباشرة",                 Message = "حصة \"تطبيق الإعراب\" تبدأ بعد 24 ساعة.",            CreatedAt = DateTime.UtcNow.AddHours(-6), IsRead = false },
                new Notification { UserId = s2.Id, Type = "ForumReply",  Title = "رد جديد على منشورك",                 Message = "قام أحد الطلاب بالرد على سؤالك في المنتدى.",         CreatedAt = DateTime.UtcNow.AddHours(-18),IsRead = false }
            };

            context.Notifications.AddRange(rows);
            await context.SaveChangesAsync();
            logger.LogInformation("Notifications seeded ({Count}).", rows.Count);
        }

        public static async Task SeedChatMessagesAsync(
            EduCoreDbContext context, UserManager<User> userManager, ILogger logger)
        {
            if (await context.ChatMessages.AnyAsync()) return;

            var s1 = await userManager.FindByEmailAsync("ali.mahmoud@educore.com");
            var s3 = await userManager.FindByEmailAsync("youssef.ibrahim@educore.com");
            if (s1 == null) return;

            var rows = new List<ChatMessage>
            {
                // English chat — student asking the AI tutor
                new ChatMessage { UserId = s1.Id, Role = "user",      Content = "Can you explain what a derivative is?",                                                          CreatedAt = DateTime.UtcNow.AddHours(-2) },
                new ChatMessage { UserId = s1.Id, Role = "assistant", Content = "A derivative measures how a function changes as its input changes — the slope at a point.",      CreatedAt = DateTime.UtcNow.AddHours(-2).AddSeconds(5) },
                new ChatMessage { UserId = s1.Id, Role = "user",      Content = "What is the chain rule?",                                                                        CreatedAt = DateTime.UtcNow.AddHours(-1) },
                new ChatMessage { UserId = s1.Id, Role = "assistant", Content = "The chain rule lets you differentiate composite functions: (f∘g)' = f'(g) · g'.",                CreatedAt = DateTime.UtcNow.AddHours(-1).AddSeconds(4) }
            };

            // Arabic chat
            if (s3 != null)
            {
                rows.Add(new ChatMessage { UserId = s3.Id, Role = "user", Content = "ممكن تشرحلي إيه الفرق بين الفعل اللازم والمتعدي؟", CreatedAt = DateTime.UtcNow.AddHours(-3) });
                rows.Add(new ChatMessage { UserId = s3.Id, Role = "assistant", Content = "الفعل اللازم لا يحتاج مفعولاً به ليُكمل معناه (مثل: نام الطفل)، أما المتعدي فيحتاج مفعولاً به (مثل: كتب الطالب الدرس).", CreatedAt = DateTime.UtcNow.AddHours(-3).AddSeconds(4) });
                rows.Add(new ChatMessage { UserId = s3.Id, Role = "user", Content = "اعطني أمثلة أكتر.", CreatedAt = DateTime.UtcNow.AddHours(-2).AddMinutes(-30) });
                rows.Add(new ChatMessage { UserId = s3.Id, Role = "assistant", Content = "أمثلة على اللازم: ضحك، جلس، ذهب. أمثلة على المتعدي: شرب الماء، قرأ الكتاب، أكل التفاحة.", CreatedAt = DateTime.UtcNow.AddHours(-2).AddMinutes(-30).AddSeconds(4) });
            }

            context.ChatMessages.AddRange(rows);
            await context.SaveChangesAsync();
            logger.LogInformation("ChatMessages seeded ({Count}).", rows.Count);
        }
    }
}