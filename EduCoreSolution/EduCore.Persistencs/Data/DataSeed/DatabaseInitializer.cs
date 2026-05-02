using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Persistencs.Data.DataSeed.Seeders;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduCore.Persistencs.Data.DataSeed
{
    public class DatabaseInitializer : IDataInitializer
    {
        private readonly EduCoreDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(
            EduCoreDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DatabaseInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("=== Database initialization started ===");

                // Apply pending migrations (safe on MonsterASP single instance)
                var pending = await _context.Database.GetPendingMigrationsAsync();
                if (pending.Any())
                {
                    _logger.LogInformation("Applying {Count} pending migration(s)...", pending.Count());
                    await _context.Database.MigrateAsync();
                }

                // Seeders run in dependency order. Each is idempotent.
                await IdentitySeeder.SeedRolesAsync(_roleManager, _logger);
                var center = await CatalogSeeder.SeedCenterAsync(_context, _logger);
                await CatalogSeeder.SeedPayoutSettingsAsync(_context, _logger);
                await IdentitySeeder.SeedUsersAsync(_userManager, center.Id, _logger);

                await CatalogSeeder.SeedCategoriesAsync(_context, center.Id, _logger);
                await CatalogSeeder.SeedCoursesAsync(_context, _userManager, _logger);
                await CatalogSeeder.SeedLessonContentAsync(_context, _logger);

                await AssessmentSeeder.SeedQuizzesAsync(_context, _logger);

                await CommerceSeeder.SeedEnrollmentsAsync(_context, _userManager, _logger);
                await CommerceSeeder.SeedPaymentsAsync(_context, _logger);
                await CommerceSeeder.SeedCashPaymentRequestsAsync(_context, _userManager, _logger);

                await ProgressSeeder.SeedLessonProgressAsync(_context, _logger);
                await ProgressSeeder.SeedAttendanceAsync(_context, _logger);
                await ProgressSeeder.SeedCertificatesAsync(_context, _logger);
                await ProgressSeeder.SeedCourseReviewsAsync(_context, _logger);

                await CommunitySeeder.SeedForumAsync(_context, _userManager, _logger);
                await CommunitySeeder.SeedNotificationsAsync(_context, _userManager, _logger);
                await CommunitySeeder.SeedChatMessagesAsync(_context, _userManager, _logger);

                await AssessmentSeeder.SeedQuizAttemptsAsync(_context, _userManager, _logger);

                await CommerceSeeder.SeedTeacherEarningsAsync(_context, _logger);
                await CommerceSeeder.SeedTeacherInvoicesAsync(_context, _logger);

                await MiscSeeder.SeedAuditLogsAsync(_context, _userManager, _logger);
                await MiscSeeder.SeedTranslationsAsync(_context, _logger);

                _logger.LogInformation("=== Database initialization completed ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database initialization failed.");
                // Don't rethrow on production: app should still start even if seed fails
                // Comment the next line if you want startup to fail-fast
                // throw;
            }
        }
    }
}