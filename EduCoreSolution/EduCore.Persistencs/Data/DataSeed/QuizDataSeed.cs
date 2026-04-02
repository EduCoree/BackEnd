using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CenterModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.QuizModel;
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
    public class QuizDataSeed
    {
        public static async Task SeedAsync(EduCoreDbContext context, UserManager<User> userManager)
        {
            // 1. Center
            if (!await context.Centers.AnyAsync())
            {
                var center = new Center
                {
                    Name = "EduCore Training Center",
                    ContactEmail = "admin@educore.com",
                    Phone = "01000000000",
                    Address = "Cairo, Egypt"
                };
                await context.Centers.AddAsync(center);
                await context.SaveChangesAsync();
            }

            var seededCenter = await context.Centers.FirstAsync();

            // 2. Teacher
            if (await userManager.FindByEmailAsync("teacher@educore.com") is null)
            {
                var teacher = new User
                {
                    UserName = "teacher@educore.com",
                    Email = "teacher@educore.com",
                    Name = "Ahmed Teacher",
                    CenterId = seededCenter.Id,
                    Role = UserRole.Teacher,
                    IsActive = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(teacher, "Teacher@123");
            }

            // 3. Student
            if (await userManager.FindByEmailAsync("student@educore.com") is null)
            {
                var student = new User
                {
                    UserName = "student@educore.com",
                    Email = "student@educore.com",
                    Name = "Ali Student",
                    CenterId = seededCenter.Id,
                    Role = UserRole.Student,
                    IsActive = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student, "Student@123");
            }

            // 4. Category
            if (!await context.Categories.AnyAsync())
            {
                var category = new Category
                {
                    CenterId = seededCenter.Id,
                    Name = "Programming",
                    Slug = "programming"
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
            }

            var seededCategory = await context.Categories.FirstAsync();
            var seededTeacher = await userManager.FindByEmailAsync("teacher@educore.com");

            // 5. Course
            if (!await context.Courses.AnyAsync())
            {
                var course = new Course
                {
                    CategoryId = seededCategory.Id,
                    TeacherId = seededTeacher!.Id,
                    Title = "C# Advanced",
                    Description = "Advanced C# course",
                    Level = CourseLevel.Advanced,
                    PricingType = CoursePricingType.Free,
                    Status = CourseStatus.Published
                };
                await context.Courses.AddAsync(course);
                await context.SaveChangesAsync();
            }
        }
    }
}
