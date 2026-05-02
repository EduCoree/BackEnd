using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.TranslationModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduCore.Persistencs.Data.DataSeed.Seeders
{
    public static class MiscSeeder
    {
        public static async Task SeedAuditLogsAsync(
            EduCoreDbContext context, UserManager<User> userManager, ILogger logger)
        {
            if (await context.AuditLogs.AnyAsync()) return;

            var admin = await userManager.FindByEmailAsync("admin@educore.com");
            var t1 = await userManager.FindByEmailAsync("ahmed.teacher@educore.com");
            if (admin == null || t1 == null) return;

            var algebra = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Algebra for Beginners");

            var rows = new List<AuditLog>
            {
                new AuditLog { UserId = admin.Id, Action = "user.create",   EntityType = "User",   EntityId = null, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new AuditLog { UserId = t1.Id,    Action = "course.create", EntityType = "Course", EntityId = algebra?.Id, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new AuditLog { UserId = t1.Id,    Action = "course.publish", EntityType = "Course", EntityId = algebra?.Id, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new AuditLog { UserId = admin.Id, Action = "settings.update", EntityType = "PayoutSettings", EntityId = 1, CreatedAt = DateTime.UtcNow.AddDays(-20) }
            };

            context.AuditLogs.AddRange(rows);
            await context.SaveChangesAsync();
            logger.LogInformation("AuditLogs seeded ({Count}).", rows.Count);
        }

        public static async Task SeedTranslationsAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.Translations.AnyAsync()) return;

            var algebra = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Algebra for Beginners");
            var calculus = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Calculus Fundamentals");
            var biology = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Advanced Biology");
            var nahw = await context.Courses.FirstOrDefaultAsync(c => c.Title == "أساسيات النحو العربي");
            var english = await context.Courses.FirstOrDefaultAsync(c => c.Title == "English Conversation B2");

            var rows = new List<Translation>();

            // English courses → Arabic translations
            if (algebra != null)
            {
                rows.Add(new Translation { EntityType = "Course", EntityId = algebra.Id, Field = "Title", Lang = "ar", Value = "الجبر للمبتدئين" });
                rows.Add(new Translation { EntityType = "Course", EntityId = algebra.Id, Field = "Description", Lang = "ar", Value = "تعلم الجبر من الصفر خطوة بخطوة" });
            }
            if (calculus != null)
            {
                rows.Add(new Translation { EntityType = "Course", EntityId = calculus.Id, Field = "Title", Lang = "ar", Value = "أساسيات التفاضل والتكامل" });
                rows.Add(new Translation { EntityType = "Course", EntityId = calculus.Id, Field = "Description", Lang = "ar", Value = "شرح المشتقات والتكاملات بأسلوب مبسط" });
            }
            if (biology != null)
            {
                rows.Add(new Translation { EntityType = "Course", EntityId = biology.Id, Field = "Title", Lang = "ar", Value = "الأحياء المتقدمة" });
                rows.Add(new Translation { EntityType = "Course", EntityId = biology.Id, Field = "Description", Lang = "ar", Value = "علم الخلية والوراثة في مستوى متقدم" });
            }
            if (english != null)
            {
                rows.Add(new Translation { EntityType = "Course", EntityId = english.Id, Field = "Title", Lang = "ar", Value = "محادثة إنجليزية مستوى B2" });
                rows.Add(new Translation { EntityType = "Course", EntityId = english.Id, Field = "Description", Lang = "ar", Value = "حسّن مهاراتك في التحدث بالإنجليزية لمستوى B2" });
            }

            // Arabic courses → English translations
            if (nahw != null)
            {
                rows.Add(new Translation { EntityType = "Course", EntityId = nahw.Id, Field = "Title", Lang = "en", Value = "Arabic Grammar Basics" });
                rows.Add(new Translation { EntityType = "Course", EntityId = nahw.Id, Field = "Description", Lang = "en", Value = "Learn Arabic grammar from scratch in a simple way" });
            }

            if (rows.Any())
            {
                context.Translations.AddRange(rows);
                await context.SaveChangesAsync();
                logger.LogInformation("Translations seeded ({Count}).", rows.Count);
            }
        }
    }
}