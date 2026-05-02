using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CenterModel;
using EduCore.Domain.Entities.ContentModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.PayoutModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduCore.Persistencs.Data.DataSeed.Seeders
{
    public static class CatalogSeeder
    {
        public static async Task<Center> SeedCenterAsync(EduCoreDbContext context, ILogger logger)
        {
            var existing = await context.Centers.FirstOrDefaultAsync();
            if (existing != null) return existing;

            var center = new Center
            {
                Name = "EduCore Academy",
                ContactEmail = "info@educore.com",
                Phone = "01000000000",
                Address = "Cairo, Egypt",
                LogoUrl = null,
                CreatedAt = DateTime.UtcNow
            };
            context.Centers.Add(center);
            await context.SaveChangesAsync();
            logger.LogInformation("Center seeded: {Name}", center.Name);
            return center;
        }

        public static async Task SeedPayoutSettingsAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.PayoutSettings.AnyAsync()) return;

            context.PayoutSettings.Add(new PayoutSettings
            {
                TeacherCommissionRate = 0.80m,
                Tier1Threshold = 10,
                Tier1Bonus = 500m,
                Tier2Threshold = 30,
                Tier2Bonus = 1500m,
                Tier3Threshold = 50,
                Tier3Bonus = 3000m,
                Currency = "EGP",
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            logger.LogInformation("PayoutSettings seeded.");
        }

        public static async Task SeedCategoriesAsync(EduCoreDbContext context, int centerId, ILogger logger)
        {
            if (await context.Categories.AnyAsync()) return;

            var now = DateTime.UtcNow;
            context.Categories.AddRange(
                new Category { CenterId = centerId, Name = "Mathematics", Slug = "mathematics", CreatedAt = now },
                new Category { CenterId = centerId, Name = "Science", Slug = "science", CreatedAt = now },
                new Category { CenterId = centerId, Name = "Arabic", Slug = "arabic", CreatedAt = now },
                new Category { CenterId = centerId, Name = "English", Slug = "english", CreatedAt = now },
                new Category { CenterId = centerId, Name = "Physics", Slug = "physics", CreatedAt = now },
                new Category { CenterId = centerId, Name = "Chemistry", Slug = "chemistry", CreatedAt = now }
            );
            await context.SaveChangesAsync();
            logger.LogInformation("Categories seeded (6).");
        }

        public static async Task SeedCoursesAsync(
            EduCoreDbContext context, UserManager<User> userManager, ILogger logger)
                {
                    if (await context.Courses.AnyAsync()) return;

                    var t1 = await userManager.FindByEmailAsync("ahmed.tawfik@educore.com");
                    var t2 = await userManager.FindByEmailAsync("ahmed.samir@educore.com");
                    var t3 = await userManager.FindByEmailAsync("menna.abulela@educore.com");
                    if (t1 == null || t2 == null || t3 == null)
                    {
                        logger.LogWarning("Teachers not found, skipping course seed.");
                        return;
                    }

                    var math = await context.Categories.FirstAsync(c => c.Slug == "mathematics");
                    var science = await context.Categories.FirstAsync(c => c.Slug == "science");
                    var arabic = await context.Categories.FirstAsync(c => c.Slug == "arabic");
                    var english = await context.Categories.FirstAsync(c => c.Slug == "english");
                    var physics = await context.Categories.FirstAsync(c => c.Slug == "physics");
                    var chem = await context.Categories.FirstAsync(c => c.Slug == "chemistry");

                    var courses = new List<Course>
            {
                // 1. English course (taught in English)
                new Course
                {
                    TeacherId = t1.Id, CategoryId = math.Id,
                    Title = "Algebra for Beginners",
                    Description = "Learn algebra from scratch step by step",
                    Level = CourseLevel.Beginner, Price = 199,
                    PricingType = CoursePricingType.Paid, Status = CourseStatus.Published,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    Sections = new List<Section>
                    {
                        new Section { Title = "Introduction to Algebra", SortOrder = 1, CreatedAt = DateTime.UtcNow.AddDays(-30), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "What is Algebra?",      SortOrder = 1, IsFreePreview = true,  DurationSeconds = 600,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                            new Lesson { Title = "Numbers and Variables", SortOrder = 2, IsFreePreview = false, DurationSeconds = 900,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                            new Lesson { Title = "Quiz Sheet",            SortOrder = 3, IsFreePreview = false, DurationSeconds = null, Type = LessonType.Pdf,   CreatedAt = DateTime.UtcNow.AddDays(-30) }
                        }},
                        new Section { Title = "Linear Equations", SortOrder = 2, CreatedAt = DateTime.UtcNow.AddDays(-29), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Solving One-Step Equations", SortOrder = 1, IsFreePreview = false, DurationSeconds = 1200, Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                            new Lesson { Title = "Live Session Recap",         SortOrder = 2, IsFreePreview = false, DurationSeconds = 3600, Type = LessonType.Live,  CreatedAt = DateTime.UtcNow.AddDays(-29) },
                            new Lesson { Title = "Practice Problems PDF",      SortOrder = 3, IsFreePreview = false, DurationSeconds = null, Type = LessonType.Pdf,   CreatedAt = DateTime.UtcNow.AddDays(-29) }
                        }}
                    }
                },
                // 2. English course
                new Course
                {
                    TeacherId = t1.Id, CategoryId = math.Id,
                    Title = "Calculus Fundamentals",
                    Description = "Derivatives and integrals explained simply",
                    Level = CourseLevel.Intermediate, Price = 0,
                    PricingType = CoursePricingType.Free, Status = CourseStatus.Published,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    Sections = new List<Section>
                    {
                        new Section { Title = "Limits", SortOrder = 1, CreatedAt = DateTime.UtcNow.AddDays(-20), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Introduction to Limits", SortOrder = 1, IsFreePreview = true,  DurationSeconds = 800,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                            new Lesson { Title = "Limit Laws",             SortOrder = 2, IsFreePreview = false, DurationSeconds = 1100, Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-20) }
                        }},
                        new Section { Title = "Derivatives", SortOrder = 2, CreatedAt = DateTime.UtcNow.AddDays(-19), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "What is a Derivative?", SortOrder = 1, IsFreePreview = false, DurationSeconds = 900,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                            new Lesson { Title = "Chain Rule",            SortOrder = 2, IsFreePreview = false, DurationSeconds = 1300, Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                            new Lesson { Title = "Derivatives Summary",   SortOrder = 3, IsFreePreview = false, DurationSeconds = null, Type = LessonType.Pdf,   CreatedAt = DateTime.UtcNow.AddDays(-19) }
                        }}
                    }
                },
                // 3. English course
                new Course
                {
                    TeacherId = t2.Id, CategoryId = science.Id,
                    Title = "Advanced Biology",
                    Description = "Cell biology and genetics at advanced level",
                    Level = CourseLevel.Advanced, Price = 349,
                    PricingType = CoursePricingType.Paid, Status = CourseStatus.Published,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    Sections = new List<Section>
                    {
                        new Section { Title = "Cell Structure", SortOrder = 1, CreatedAt = DateTime.UtcNow.AddDays(-15), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Cell Membrane",    SortOrder = 1, IsFreePreview = true,  DurationSeconds = 700,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                            new Lesson { Title = "Mitochondria",     SortOrder = 2, IsFreePreview = false, DurationSeconds = 850,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                            new Lesson { Title = "Live Q&A Session", SortOrder = 3, IsFreePreview = false, DurationSeconds = 5400, Type = LessonType.Live,  CreatedAt = DateTime.UtcNow.AddDays(-15) }
                        }},
                        new Section { Title = "Genetics", SortOrder = 2, CreatedAt = DateTime.UtcNow.AddDays(-14), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "DNA Structure",  SortOrder = 1, IsFreePreview = false, DurationSeconds = 1000, Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                            new Lesson { Title = "Genetics Notes", SortOrder = 2, IsFreePreview = false, DurationSeconds = null, Type = LessonType.Pdf,   CreatedAt = DateTime.UtcNow.AddDays(-14) }
                        }}
                    }
                },
                // 4. Arabic course — Arabic Grammar
                new Course
                {
                    TeacherId = t3.Id, CategoryId = arabic.Id,
                    Title = "أساسيات النحو العربي",
                    Description = "تعلم قواعد اللغة العربية من البداية بأسلوب مبسط",
                    Level = CourseLevel.Beginner, Price = 99,
                    PricingType = CoursePricingType.Subscription, Status = CourseStatus.Published,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    Sections = new List<Section>
                    {
                        new Section { Title = "النحو الأساسي", SortOrder = 1, CreatedAt = DateTime.UtcNow.AddDays(-10), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "المبتدأ والخبر",    SortOrder = 1, IsFreePreview = true,  DurationSeconds = 600,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                            new Lesson { Title = "الفاعل والمفعول",   SortOrder = 2, IsFreePreview = false, DurationSeconds = 800,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                            new Lesson { Title = "ملخص قواعد النحو",  SortOrder = 3, IsFreePreview = false, DurationSeconds = null, Type = LessonType.Pdf,   CreatedAt = DateTime.UtcNow.AddDays(-10) }
                        }},
                        new Section { Title = "الإعراب", SortOrder = 2, CreatedAt = DateTime.UtcNow.AddDays(-9), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "علامات الإعراب الأصلية",  SortOrder = 1, IsFreePreview = false, DurationSeconds = 900,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                            new Lesson { Title = "حصة مباشرة: تطبيق إعراب", SortOrder = 2, IsFreePreview = false, DurationSeconds = 3600, Type = LessonType.Live,  CreatedAt = DateTime.UtcNow.AddDays(-9) }
                        }}
                    }
                },
                // 5. English course (teaches English — stays English)
                new Course
                {
                    TeacherId = t2.Id, CategoryId = english.Id,
                    Title = "English Conversation B2",
                    Description = "Improve your spoken English to B2 level",
                    Level = CourseLevel.Intermediate, Price = 249,
                    PricingType = CoursePricingType.Paid, Status = CourseStatus.Published,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    Sections = new List<Section>
                    {
                        new Section { Title = "Daily Conversations", SortOrder = 1, CreatedAt = DateTime.UtcNow.AddDays(-5), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Greetings and Small Talk", SortOrder = 1, IsFreePreview = true,  DurationSeconds = 500,  Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                            new Lesson { Title = "Live Speaking Practice",   SortOrder = 2, IsFreePreview = false, DurationSeconds = 3600, Type = LessonType.Live,  CreatedAt = DateTime.UtcNow.AddDays(-5) }
                        }},
                        new Section { Title = "Business English", SortOrder = 2, CreatedAt = DateTime.UtcNow.AddDays(-4), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Emails and Reports", SortOrder = 1, IsFreePreview = false, DurationSeconds = 1100, Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-4) },
                            new Lesson { Title = "Business Vocab PDF", SortOrder = 2, IsFreePreview = false, DurationSeconds = null, Type = LessonType.Pdf,   CreatedAt = DateTime.UtcNow.AddDays(-4) }
                        }}
                    }
                },
                // 6. Arabic course — Physics in Arabic
                new Course
                {
                    TeacherId = t3.Id, CategoryId = physics.Id,
                    Title = "الفيزياء: الميكانيكا (مسودة)",
                    Description = "قيد الإعداد — قوانين نيوتن والحركة",
                    Level = CourseLevel.Advanced, Price = 299,
                    PricingType = CoursePricingType.Paid, Status = CourseStatus.Draft,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Sections = new List<Section>
                    {
                        new Section { Title = "قوانين نيوتن", SortOrder = 1, CreatedAt = DateTime.UtcNow.AddDays(-2), Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "القانون الأول",  SortOrder = 1, IsFreePreview = false, DurationSeconds = 700, Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                            new Lesson { Title = "القانون الثاني", SortOrder = 2, IsFreePreview = false, DurationSeconds = 900, Type = LessonType.Video, CreatedAt = DateTime.UtcNow.AddDays(-2) }
                        }}
                    }
                },
                // 7. Arabic course — Chemistry in Arabic (archived)
                new Course
                {
                    TeacherId = t1.Id, CategoryId = chem.Id,
                    Title = "أساسيات الكيمياء (مؤرشف)",
                    Description = "كورس قديم — لم يعد متاحاً",
                    Level = CourseLevel.Beginner, Price = 150,
                    PricingType = CoursePricingType.Paid, Status = CourseStatus.Archived,
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    Sections = new List<Section>()
                }
            };

                    context.Courses.AddRange(courses);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Courses seeded ({Count}) with sections and lessons.", courses.Count);
        }

        /// <summary>
        /// Seeds VideoLesson / PdfLesson / LiveSession rows for every Lesson based on its Type.
        /// Uses real YouTube educational videos matched to each lesson's topic.
        /// </summary>
        public static async Task SeedLessonContentAsync(EduCoreDbContext context, ILogger logger)
        {
            // Lesson title → real YouTube video URL (educational content matching the topic)
            // If a lesson title isn't in this dictionary, a generic fallback is used.
            var videoMap = new Dictionary<string, (string Url, string Thumb, string? Transcript)>
            {
                // ── Algebra for Beginners ─────────────────────────────────────────
                ["What is Algebra?"] = (
                    "https://www.youtube.com/watch?v=NybHckSEQBI",
                    "https://img.youtube.com/vi/NybHckSEQBI/hqdefault.jpg",
                    "Algebra is the branch of mathematics that uses letters and symbols to represent numbers..."),
                ["Numbers and Variables"] = (
                    "https://www.youtube.com/watch?v=vDqOoI-4Z6M",
                    "https://img.youtube.com/vi/vDqOoI-4Z6M/hqdefault.jpg",
                    null),
                ["Solving One-Step Equations"] = (
                    "https://www.youtube.com/watch?v=9DxrF6Ttws4",
                    "https://img.youtube.com/vi/9DxrF6Ttws4/hqdefault.jpg",
                    null),

                // ── Calculus Fundamentals ─────────────────────────────────────────
                ["Introduction to Limits"] = (
                    "https://www.youtube.com/watch?v=riXcZT2ICjA",
                    "https://img.youtube.com/vi/riXcZT2ICjA/hqdefault.jpg",
                    null),
                ["Limit Laws"] = (
                    "https://www.youtube.com/watch?v=YNstP0ESndU",
                    "https://img.youtube.com/vi/YNstP0ESndU/hqdefault.jpg",
                    null),
                ["What is a Derivative?"] = (
                    "https://www.youtube.com/watch?v=WUvTyaaNkzM",
                    "https://img.youtube.com/vi/WUvTyaaNkzM/hqdefault.jpg",
                    "The derivative measures the instantaneous rate of change of a function..."),
                ["Chain Rule"] = (
                    "https://www.youtube.com/watch?v=H-ybCx8gt-8",
                    "https://img.youtube.com/vi/H-ybCx8gt-8/hqdefault.jpg",
                    null),

                // ── Advanced Biology ──────────────────────────────────────────────
                ["Cell Membrane"] = (
                    "https://www.youtube.com/watch?v=moPJkCbKjBs",
                    "https://img.youtube.com/vi/moPJkCbKjBs/hqdefault.jpg",
                    null),
                ["Mitochondria"] = (
                    "https://www.youtube.com/watch?v=RrS2uROUjK4",
                    "https://img.youtube.com/vi/RrS2uROUjK4/hqdefault.jpg",
                    null),
                ["DNA Structure"] = (
                    "https://www.youtube.com/watch?v=zwibgNGe4aY",
                    "https://img.youtube.com/vi/zwibgNGe4aY/hqdefault.jpg",
                    null),

                // ── أساسيات النحو العربي ───────────────────────────────────────────
                ["المبتدأ والخبر"] = (
                    "https://www.youtube.com/watch?v=8u6L-rBkEHE",
                    "https://img.youtube.com/vi/8u6L-rBkEHE/hqdefault.jpg",
                    "المبتدأ هو الاسم المرفوع في أول الجملة الاسمية، والخبر هو ما يكمل معنى المبتدأ..."),
                ["الفاعل والمفعول"] = (
                    "https://www.youtube.com/watch?v=8O0ZTfA1lBY",
                    "https://img.youtube.com/vi/8O0ZTfA1lBY/hqdefault.jpg",
                    null),
                ["علامات الإعراب الأصلية"] = (
                    "https://www.youtube.com/watch?v=5MgBikgcWnY",
                    "https://img.youtube.com/vi/5MgBikgcWnY/hqdefault.jpg",
                    null),

                // ── English Conversation B2 ───────────────────────────────────────
                ["Greetings and Small Talk"] = (
                    "https://www.youtube.com/watch?v=Bd8I0bnDk6Y",
                    "https://img.youtube.com/vi/Bd8I0bnDk6Y/hqdefault.jpg",
                    null),
                ["Emails and Reports"] = (
                    "https://www.youtube.com/watch?v=Yh3PbiR1jL4",
                    "https://img.youtube.com/vi/Yh3PbiR1jL4/hqdefault.jpg",
                    null),

                // ── الفيزياء: الميكانيكا ───────────────────────────────────────────
                ["القانون الأول"] = (
                    "https://www.youtube.com/watch?v=CQYELiTtUs8",
                    "https://img.youtube.com/vi/CQYELiTtUs8/hqdefault.jpg",
                    null),
                ["القانون الثاني"] = (
                    "https://www.youtube.com/watch?v=xzA6IBWUEDE",
                    "https://img.youtube.com/vi/xzA6IBWUEDE/hqdefault.jpg",
                    null),
            };

            // Generic fallback for any lesson not in the map
            var fallback = (
                Url: "https://www.youtube.com/watch?v=NybHckSEQBI",
                Thumb: "https://img.youtube.com/vi/NybHckSEQBI/hqdefault.jpg",
                Transcript: (string?)null);

            var lessons = await context.Lessons
                .Include(l => l.Section)
                .ToListAsync();

            int videoCount = 0, pdfCount = 0, liveCount = 0;

            foreach (var lesson in lessons)
            {
                // ── VIDEO content ────────────────────────────────────────────────
                if (lesson.Type.HasFlag(LessonType.Video) &&
                    !await context.VideoLessons.AnyAsync(v => v.LessonId == lesson.Id))
                {
                    var (url, thumb, transcript) = videoMap.TryGetValue(lesson.Title, out var match)
                        ? match
                        : fallback;

                    context.VideoLessons.Add(new VideoLesson
                    {
                        LessonId = lesson.Id,
                        VideoUrl = url,
                        VideoProvider = "youtube",
                        ThumbnailUrl = thumb,
                        Transcript = transcript,
                        TranscribedAt = transcript != null ? DateTime.UtcNow.AddDays(-1) : null
                    });
                    videoCount++;
                }

                // ── PDF content ──────────────────────────────────────────────────
                if (lesson.Type.HasFlag(LessonType.Pdf) &&
                    !await context.PdfLessons.AnyAsync(p => p.LessonId == lesson.Id))
                {
                    context.PdfLessons.Add(new PdfLesson
                    {
                        LessonId = lesson.Id,
                        FileUrl = $"https://cdn.educore.com/pdfs/lesson-{lesson.Id}.pdf",
                        FileSizeKb = 1024
                    });
                    pdfCount++;
                }

                // ── LIVE session content ─────────────────────────────────────────
                if (lesson.Type.HasFlag(LessonType.Live) &&
                    !await context.LiveSessions.AnyAsync(s => s.LessonId == lesson.Id))
                {
                    context.LiveSessions.Add(new LiveSession
                    {
                        CourseId = lesson.Section.CourseId,
                        LessonId = lesson.Id,
                        Provider = LiveProvider.Zoom,
                        MeetingUrl = $"https://zoom.us/j/lesson-{lesson.Id}",
                        ScheduledAt = DateTime.UtcNow.AddDays(7),
                        Title = lesson.Title,
                        Description = "Auto-generated live session",
                        CreatedAt = DateTime.UtcNow
                    });
                    liveCount++;
                }
            }

            if (videoCount + pdfCount + liveCount > 0)
            {
                await context.SaveChangesAsync();
                logger.LogInformation("Lesson content seeded — Video: {V}, Pdf: {P}, Live: {L}",
                    videoCount, pdfCount, liveCount);
            }
        }
    }
}