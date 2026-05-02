using EduCore.Domain.Entities.AuthModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EduCore.Persistencs.Data.DataSeed.Seeders
{
    public static class IdentitySeeder
    {
        public static readonly string[] Roles = { "SuperAdmin", "Admin", "Teacher", "Student" };

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Role created: {Role}", role);
                }
            }
        }

        public static async Task SeedUsersAsync(UserManager<User> userManager, int centerId, ILogger logger)
        {
            // Admin
            await CreateAsync(userManager, logger, "admin@educore.com", "هاله مدحت", centerId,
                UserRole.Admin, "Admin", "Admin@12345");

            // Teachers (3)
            await CreateAsync(userManager, logger, "ahmed.tawfik@educore.com", "أحمد توفيق", centerId,
                UserRole.Teacher, "Teacher", "Teacher@12345");
            await CreateAsync(userManager, logger, "ahmed.samir@educore.com", "أحمد سمير", centerId,
                UserRole.Teacher, "Teacher", "Teacher@12345");
            await CreateAsync(userManager, logger, "menna.abulela@educore.com", "منه أبو العلا", centerId,
                UserRole.Teacher, "Teacher", "Teacher@12345");

            // Students (5)
            await CreateAsync(userManager, logger, "ali.mahmoud@educore.com", "علي محمود", centerId,
                UserRole.Student, "Student", "Student@12345");
            await CreateAsync(userManager, logger, "nour.hassan@educore.com", "نور حسن", centerId,
                UserRole.Student, "Student", "Student@12345");
            await CreateAsync(userManager, logger, "youssef.ibrahim@educore.com", "يوسف إبراهيم", centerId,
                UserRole.Student, "Student", "Student@12345");
            await CreateAsync(userManager, logger, "layla.abdullah@educore.com", "ليلى عبدالله", centerId,
                UserRole.Student, "Student", "Student@12345");
            await CreateAsync(userManager, logger, "karim.khaled@educore.com", "كريم خالد", centerId,
                UserRole.Student, "Student", "Student@12345");
        }

        private static async Task CreateAsync(
            UserManager<User> userManager, ILogger logger,
            string email, string name, int centerId, UserRole role, string identityRole, string password)
        {
            if (await userManager.FindByEmailAsync(email) != null) return;

            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Name = name,
                CenterId = centerId,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, identityRole);
                logger.LogInformation("User created: {Email} ({Role}) - {Name}", email, identityRole, name);
            }
            else
            {
                logger.LogError("Failed to create {Email}: {Errors}", email,
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}