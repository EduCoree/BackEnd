using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CenterModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestionType = EduCore.Shared.Enums.QuestionType;

namespace EduCore.Persistencs.Data.DataSeed
{
    public class QuizDataSeed
    {

        public static async Task SeedAsync(EduCoreDbContext context, UserManager<User> userManager)
        {
            // 1. Center
            if (!await context.Centers.AnyAsync())
            {
                await context.Centers.AddAsync(new Center
                {
                    Name = "EduCore Center",
                    ContactEmail = "admin@educore.com",
                    Phone = "01000000000",
                    Address = "Cairo, Egypt"
                });
                await context.SaveChangesAsync();
            }
            var center = await context.Centers.FirstAsync();

            // 2. Teacher
            if (await userManager.FindByEmailAsync("teacher@educore.com") is null)
            {
                await userManager.CreateAsync(new User
                {
                    UserName = "teacher@educore.com",
                    Email = "teacher@educore.com",
                    Name = "Ahmed Teacher",
                    CenterId = center.Id,
                    Role = UserRole.Teacher,
                    IsActive = true,
                    EmailConfirmed = true
                }, "Teacher@123");
            }

            // 3. Student — نفس الـ Id الـ hardcoded في الـ controller
            if (await userManager.FindByIdAsync("2721eab6-9c64-404e-9911-3850dbefb12f") is null)
            {
                var studentt = new User
                {
                    Id = "2721eab6-9c64-404e-9911-3850dbefb12f",
                    UserName = "student@educore.com",
                    Email = "student@educore.com",
                    Name = "Ali Student",
                    CenterId = center.Id,
                    Role = UserRole.Student,
                    IsActive = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(studentt, "Student@123");
            }

            var teacher = await userManager.FindByEmailAsync("teacher@educore.com");
            var student = await userManager.FindByIdAsync("2721eab6-9c64-404e-9911-3850dbefb12f");

            // 4. Category
            if (!await context.Categories.AnyAsync())
            {
                await context.Categories.AddAsync(new Category
                {
                    CenterId = center.Id,
                    Name = "Programming",
                    Slug = "programming"
                });
                await context.SaveChangesAsync();
            }
            var category = await context.Categories.FirstAsync();

            // 5. Course
            if (!await context.Courses.AnyAsync())
            {
                await context.Courses.AddAsync(new Course
                {
                    CategoryId = category.Id,
                    TeacherId = teacher!.Id,
                    Title = "C# Advanced",
                    Description = "Advanced C# course",
                    Level = CourseLevel.Advanced,
                    PricingType = CoursePricingType.Free,
                    Status = CourseStatus.Published
                });
                await context.SaveChangesAsync();
            }
            var course = await context.Courses.FirstAsync();

            // 6. Enrollment
            if (!await context.Enrollments.AnyAsync())
            {
                await context.Enrollments.AddAsync(new Enrollment
                {
                    StudentId = student!.Id,
                    CourseId = course.Id,
                    Type = EnrollmentType.Free,
                    Status = EnrollmentStatus.Active,
                    EnrolledAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }

            // 7. Quiz
            if (!await context.Quizzes.AnyAsync())
            {
                await context.Quizzes.AddAsync(new Quiz
                {
                    CourseId = course.Id,
                    Title = "C# Basics Quiz",
                    TimeLimitMins = 30,
                    PassScore = 70,
                    MaxAttempts = 3,
                    IsRandomized = false
                });
                await context.SaveChangesAsync();
            }
            var quiz = await context.Quizzes.FirstAsync();

            // 8. Questions + Answer Options
            if (!await context.Questions.AnyAsync())
            {
                var q1 = new Question
                {
                    QuizId = quiz.Id,
                    Text = "What is C#?",
                    Type = QuestionType.MCQ,
                    Points = 10,
                    AnswerOptions = new List<AnswerOption>
                {
                    new() { Text = "A programming language", IsCorrect = true },
                    new() { Text = "A database", IsCorrect = false },
                    new() { Text = "An operating system", IsCorrect = false },
                    new() { Text = "A framework", IsCorrect = false }
                }
                };

                var q2 = new Question
                {
                    QuizId = quiz.Id,
                    Text = "C# is object-oriented.",
                    Type = QuestionType.TrueFalse,
                    Points = 5,
                    AnswerOptions = new List<AnswerOption>
                {
                    new() { Text = "True", IsCorrect = true },
                    new() { Text = "False", IsCorrect = false }
                }
                };

                var q3 = new Question
                {
                    QuizId = quiz.Id,
                    Text = "Which keyword is used to define a class?",
                    Type = QuestionType.MCQ,
                    Points = 10,
                    AnswerOptions = new List<AnswerOption>
                {
                    new() { Text = "class", IsCorrect = true },
                    new() { Text = "struct", IsCorrect = false },
                    new() { Text = "interface", IsCorrect = false },
                    new() { Text = "enum", IsCorrect = false }
                }
                };

                await context.Questions.AddRangeAsync(q1, q2, q3);
                await context.SaveChangesAsync();
            }
        }
    }
}
