using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CenterModel;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.DataSeed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            EduCoreDbContext context)
        {
            //  1. Roles 
            string[] roles = ["SuperAdmin", "Admin", "Teacher", "Student"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Center 
            if (!await context.Set<Center>().AnyAsync())
            {
                context.Set<Center>().AddRange(
                    new Center
                    {
                        Name = "EduCore Academy",
                        ContactEmail = "info@educore.com",
                        Phone = "01000000000",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Center
                    {
                        Name = "Future Leaders",
                        ContactEmail = "info@futureleaders.com",
                        Phone = "01111111111",
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }

            var center1 = await context.Set<Center>()
                .FirstOrDefaultAsync(c => c.Name == "EduCore Academy") ?? await context.Set<Center>().FirstOrDefaultAsync();
            var center2 = await context.Set<Center>()
                .FirstOrDefaultAsync(c => c.Name == "Future Leaders") ?? center1;

            // 3. Users 
            // Admin
            await SeedUserAsync(userManager, new User
            {
                UserName = "admin@educore.com",
                Email = "admin@educore.com",
                Name = "Super Admin",
                CenterId = center1.Id,
                IsActive = true
            }, "Admin@12345", "Admin");

            // Teachers 
            await SeedUserAsync(userManager, new User
            {
                UserName = "ahmed.teacher@educore.com",
                Email = "ahmed.teacher@educore.com",
                Name = "Ahmed Hassan",
                CenterId = center1.Id,
                IsActive = true
            }, "Teacher@12345", "Teacher");

            await SeedUserAsync(userManager, new User
            {
                UserName = "sara.teacher@educore.com",
                Email = "sara.teacher@educore.com",
                Name = "Sara Mohamed",
                CenterId = center1.Id,
                IsActive = true
            }, "Teacher@12345", "Teacher");

            await SeedUserAsync(userManager, new User
            {
                UserName = "omar.teacher@educore.com",
                Email = "omar.teacher@educore.com",
                Name = "Omar Ali",
                CenterId = center2.Id,
                IsActive = true
            }, "Teacher@12345", "Teacher");

            // Students — 
            await SeedUserAsync(userManager, new User
            {
                UserName = "student1@educore.com",
                Email = "student1@educore.com",
                Name = "Mohamed Student",
                CenterId = center1.Id,
                IsActive = true
            }, "Student@12345", "Student");

            await SeedUserAsync(userManager, new User
            {
                UserName = "student2@educore.com",
                Email = "student2@educore.com",
                Name = "Nour Student",
                CenterId = center1.Id,
                IsActive = true
            }, "Student@12345", "Student");

            await SeedUserAsync(userManager, new User
            {
                UserName = "student3@educore.com",
                Email = "student3@educore.com",
                Name = "Youssef Student",
                CenterId = center2.Id,
                IsActive = true
            }, "Student@12345", "Student");

            // 4. Categories 
            if (!await context.Set<Category>().AnyAsync())
            {
                context.Set<Category>().AddRange(
                    new Category { CenterId = center1.Id, Name = "Mathematics", Slug = "mathematics" },
                    new Category { CenterId = center1.Id, Name = "Science", Slug = "science" },
                    new Category { CenterId = center1.Id, Name = "Arabic", Slug = "arabic" },
                    new Category { CenterId = center1.Id, Name = "English", Slug = "english" },
                    new Category { CenterId = center2.Id, Name = "Physics", Slug = "physics" },
                    new Category { CenterId = center2.Id, Name = "Chemistry", Slug = "chemistry" }
                );
                await context.SaveChangesAsync();
            }

            // 5. Courses 
            if (!await context.Set<Course>().AnyAsync())
            {
                var teacher1 = await userManager.FindByEmailAsync("ahmed.teacher@educore.com");
                var teacher2 = await userManager.FindByEmailAsync("sara.teacher@educore.com");
                var teacher3 = await userManager.FindByEmailAsync("omar.teacher@educore.com");

                var mathCat = await context.Set<Category>().FirstAsync(c => c.Slug == "mathematics");
                var sciCat = await context.Set<Category>().FirstAsync(c => c.Slug == "science");
                var arabicCat = await context.Set<Category>().FirstAsync(c => c.Slug == "arabic");
                var engCat = await context.Set<Category>().FirstAsync(c => c.Slug == "english");
                var physCat = await context.Set<Category>().FirstAsync(c => c.Slug == "physics");
                var chemCat = await context.Set<Category>().FirstAsync(c => c.Slug == "chemistry");

                var courses = new List<Course>
                {
                    // Beginner + Paid 
                    new Course
                    {
                        TeacherId   = teacher1!.Id,
                        CategoryId  = mathCat.Id,
                        Title       = "Algebra for Beginners",
                        Description = "Learn algebra from scratch step by step",
                        Level       = CourseLevel.Beginner,
                        Price       = 199,
                        PricingType = CoursePricingType.Paid,
                        Status      = CourseStatus.Published,
                        CreatedAt   = DateTime.UtcNow.AddDays(-30),
                        Sections    =
                        [
                            new Section
                            {
                                Title     = "Introduction to Algebra",
                                SortOrder = 1,
                                Lessons   =
                                [
                                    new Lesson { Title = "What is Algebra?",     SortOrder = 1, IsFreePreview = true,  DurationSeconds = 600,  Type = LessonType.Video },
                                    new Lesson { Title = "Numbers and Variables", SortOrder = 2, IsFreePreview = false, DurationSeconds = 900,  Type = LessonType.Video },
                                    new Lesson { Title = "Quiz Sheet",            SortOrder = 3, IsFreePreview = false, DurationSeconds = null,  Type = LessonType.Pdf   }
                                ]
                            },
                            new Section
                            {
                                Title     = "Linear Equations",
                                SortOrder = 2,
                                Lessons   =
                                [
                                    new Lesson { Title = "Solving One-Step Equations", SortOrder = 1, IsFreePreview = false, DurationSeconds = 1200, Type = LessonType.Video },
                                    new Lesson { Title = "Live Session Recap",         SortOrder = 2, IsFreePreview = false, DurationSeconds = 3600, Type = LessonType.Live  },
                                    new Lesson { Title = "Practice Problems PDF",      SortOrder = 3, IsFreePreview = false, DurationSeconds = null,  Type = LessonType.Pdf   }
                                ]
                            }
                        ]
                    },

                    // Intermediate + Free 
                    new Course
                    {
                        TeacherId   = teacher1!.Id,
                        CategoryId  = mathCat.Id,
                        Title       = "Calculus Fundamentals",
                        Description = "Derivatives and integrals explained simply",
                        Level       = CourseLevel.Intermediate,
                        Price       = 0,
                        PricingType = CoursePricingType.Free,
                        Status      = CourseStatus.Published,
                        CreatedAt   = DateTime.UtcNow.AddDays(-20),
                        Sections    =
                        [
                            new Section
                            {
                                Title     = "Limits",
                                SortOrder = 1,
                                Lessons   =
                                [
                                    new Lesson { Title = "Introduction to Limits", SortOrder = 1, IsFreePreview = true,  DurationSeconds = 800,  Type = LessonType.Video },
                                    new Lesson { Title = "Limit Laws",             SortOrder = 2, IsFreePreview = false, DurationSeconds = 1100, Type = LessonType.Video }
                                ]
                            },
                            new Section
                            {
                                Title     = "Derivatives",
                                SortOrder = 2,
                                Lessons   =
                                [
                                    new Lesson { Title = "What is a Derivative?", SortOrder = 1, IsFreePreview = false, DurationSeconds = 900,  Type = LessonType.Video },
                                    new Lesson { Title = "Chain Rule",            SortOrder = 2, IsFreePreview = false, DurationSeconds = 1300, Type = LessonType.Video },
                                    new Lesson { Title = "Derivatives Summary",   SortOrder = 3, IsFreePreview = false, DurationSeconds = null,  Type = LessonType.Pdf   }
                                ]
                            }
                        ]
                    },

                    //  Advanced + Paid
                    new Course
                    {
                        TeacherId   = teacher2!.Id,
                        CategoryId  = sciCat.Id,
                        Title       = "Advanced Biology",
                        Description = "Cell biology and genetics at advanced level",
                        Level       = CourseLevel.Advanced,
                        Price       = 349,
                        PricingType = CoursePricingType.Paid,
                        Status      = CourseStatus.Published,
                        CreatedAt   = DateTime.UtcNow.AddDays(-15),
                        Sections    =
                        [
                            new Section
                            {
                                Title     = "Cell Structure",
                                SortOrder = 1,
                                Lessons   =
                                [
                                    new Lesson { Title = "Cell Membrane",   SortOrder = 1, IsFreePreview = true,  DurationSeconds = 700,  Type = LessonType.Video },
                                    new Lesson { Title = "Mitochondria",    SortOrder = 2, IsFreePreview = false, DurationSeconds = 850,  Type = LessonType.Video },
                                    new Lesson { Title = "Live Q&A Session",SortOrder = 3, IsFreePreview = false, DurationSeconds = 5400, Type = LessonType.Live  }
                                ]
                            },
                            new Section
                            {
                                Title     = "Genetics",
                                SortOrder = 2,
                                Lessons   =
                                [
                                    new Lesson { Title = "DNA Structure",   SortOrder = 1, IsFreePreview = false, DurationSeconds = 1000, Type = LessonType.Video },
                                    new Lesson { Title = "Genetics Notes",  SortOrder = 2, IsFreePreview = false, DurationSeconds = null,  Type = LessonType.Pdf   }
                                ]
                            }
                        ]
                    },

                    //  Beginner + Subscription
                    new Course
                    {
                        TeacherId   = teacher2!.Id,
                        CategoryId  = arabicCat.Id,
                        Title       = "Arabic Grammar Basics",
                        Description = "Master Arabic grammar from zero",
                        Level       = CourseLevel.Beginner,
                        Price       = 99,
                        PricingType = CoursePricingType.Subscription,
                        Status      = CourseStatus.Published,
                        CreatedAt   = DateTime.UtcNow.AddDays(-10),
                        Sections    =
                        [
                            new Section
                            {
                                Title     = "النحو الأساسي",
                                SortOrder = 1,
                                Lessons   =
                                [
                                    new Lesson { Title = "المبتدأ والخبر",  SortOrder = 1, IsFreePreview = true,  DurationSeconds = 600,  Type = LessonType.Video },
                                    new Lesson { Title = "الفاعل والمفعول", SortOrder = 2, IsFreePreview = false, DurationSeconds = 800,  Type = LessonType.Video },
                                    new Lesson { Title = "ملخص النحو",      SortOrder = 3, IsFreePreview = false, DurationSeconds = null,  Type = LessonType.Pdf   }
                                ]
                            }
                        ]
                    },

                    // Intermediate + Paid 
                    new Course
                    {
                        TeacherId   = teacher3!.Id,
                        CategoryId  = engCat.Id,
                        Title       = "English Conversation B2",
                        Description = "Improve your spoken English to B2 level",
                        Level       = CourseLevel.Intermediate,
                        Price       = 249,
                        PricingType = CoursePricingType.Paid,
                        Status      = CourseStatus.Published,
                        CreatedAt   = DateTime.UtcNow.AddDays(-5),
                        Sections    =
                        [
                            new Section
                            {
                                Title     = "Daily Conversations",
                                SortOrder = 1,
                                Lessons   =
                                [
                                    new Lesson { Title = "Greetings and Small Talk", SortOrder = 1, IsFreePreview = true,  DurationSeconds = 500,  Type = LessonType.Video },
                                    new Lesson { Title = "Live Speaking Practice",   SortOrder = 2, IsFreePreview = false, DurationSeconds = 3600, Type = LessonType.Live  }
                                ]
                            },
                            new Section
                            {
                                Title     = "Business English",
                                SortOrder = 2,
                                Lessons   =
                                [
                                    new Lesson { Title = "Emails and Reports", SortOrder = 1, IsFreePreview = false, DurationSeconds = 1100, Type = LessonType.Video },
                                    new Lesson { Title = "Business Vocab PDF", SortOrder = 2, IsFreePreview = false, DurationSeconds = null,  Type = LessonType.Pdf   }
                                ]
                            }
                        ]
                    },

                    //  Draft — not for public
                    new Course
                    {
                        TeacherId   = teacher3!.Id,
                        CategoryId  = physCat.Id,
                        Title       = "Physics: Mechanics (Draft)",
                        Description = "Under construction",
                        Level       = CourseLevel.Advanced,
                        Price       = 299,
                        PricingType = CoursePricingType.Paid,
                        Status      = CourseStatus.Draft,      
                        CreatedAt   = DateTime.UtcNow.AddDays(-2),
                        Sections    =
                        [
                            new Section
                            {
                                Title     = "Newton's Laws",
                                SortOrder = 1,
                                Lessons   =
                                [
                                    new Lesson { Title = "First Law",  SortOrder = 1, IsFreePreview = false, DurationSeconds = 700, Type = LessonType.Video },
                                    new Lesson { Title = "Second Law", SortOrder = 2, IsFreePreview = false, DurationSeconds = 900, Type = LessonType.Video }
                                ]
                            }
                        ]
                    },

                    // ── Archived ──────────────────────────────────
                    new Course
                    {
                        TeacherId   = teacher1!.Id,
                        CategoryId  = chemCat.Id,
                        Title       = "Chemistry Basics (Archived)",
                        Description = "Old course — no longer active",
                        Level       = CourseLevel.Beginner,
                        Price       = 150,
                        PricingType = CoursePricingType.Paid,
                        Status      = CourseStatus.Archived,  
                        CreatedAt   = DateTime.UtcNow.AddDays(-60),
                        Sections    = []
                    }
                };

                context.Set<Course>().AddRange(courses);
                await context.SaveChangesAsync();
            }

            // 6. Enrollments
            if (!await context.Set<Enrollment>().AnyAsync())
            {
                var student1 = await userManager.FindByEmailAsync("student1@educore.com");

                var algebraCourse = await context.Set<Course>().FirstOrDefaultAsync(c => c.Title == "Algebra for Beginners");
                var advancedBiologyCourse = await context.Set<Course>().FirstOrDefaultAsync(c => c.Title == "Advanced Biology");

                var enrollments = new List<Enrollment>();

                if (student1 != null && algebraCourse != null)
                {
                    enrollments.Add(new Enrollment
                    {
                        StudentId = student1.Id,
                        CourseId = algebraCourse.Id,
                        Type = EnrollmentType.Free,
                        EnrolledAt = DateTime.UtcNow,
                        Status = EnrollmentStatus.Active
                    });
                }

                if (student1 != null && advancedBiologyCourse != null)
                {
                    enrollments.Add(new Enrollment
                    {
                        StudentId = student1.Id,
                        CourseId = advancedBiologyCourse.Id,
                        Type = EnrollmentType.Purchase,
                        EnrolledAt = DateTime.UtcNow,
                        Status = EnrollmentStatus.Active
                    });
                }

                if (enrollments.Any())
                {
                    context.Set<Enrollment>().AddRange(enrollments);
                    await context.SaveChangesAsync();
                }
            }

            // 7. Forum Posts
            if (!await context.Set<EduCore.Domain.Entities.ForumModel.ForumPost>().AnyAsync())
            {
                var algebraLesson = await context.Set<Lesson>().FirstOrDefaultAsync(l => l.Title == "What is Algebra?");
                var biologyLesson = await context.Set<Lesson>().FirstOrDefaultAsync(l => l.Title == "Cell Membrane");
                
                var student1 = await userManager.FindByEmailAsync("student1@educore.com");
                var student2 = await userManager.FindByEmailAsync("student2@educore.com");
                var teacher1 = await userManager.FindByEmailAsync("ahmed.teacher@educore.com");

                if (algebraLesson != null && student1 != null && student2 != null && teacher1 != null)
                {
                    var posts = new List<EduCore.Domain.Entities.ForumModel.ForumPost>
                    {
                        new EduCore.Domain.Entities.ForumModel.ForumPost
                        {
                            LessonId = algebraLesson.Id,
                            StudentId = student1.Id,
                            Title = "Help with algebraic variables",
                            Body = "I'm having trouble understanding how variables work. Can someone give an example?",
                            CreatedAt = DateTime.UtcNow.AddDays(-2),
                            UpvoteCount = 2,
                            Replies = new List<EduCore.Domain.Entities.ForumModel.ForumReply>
                            {
                                new EduCore.Domain.Entities.ForumModel.ForumReply
                                {
                                    UserId = student2.Id,
                                    Body = "Variables are like empty boxes. You can put different numbers in them!",
                                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                                },
                                new EduCore.Domain.Entities.ForumModel.ForumReply
                                {
                                    UserId = teacher1.Id,
                                    Body = "Great explanation! Think of x as a placeholder for a value we want to find.",
                                    CreatedAt = DateTime.UtcNow.AddHours(-12)
                                }
                            },
                            Upvotes = new List<EduCore.Domain.Entities.ForumModel.PostUpvote>
                            {
                                new EduCore.Domain.Entities.ForumModel.PostUpvote { UserId = student2.Id, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                                new EduCore.Domain.Entities.ForumModel.PostUpvote { UserId = teacher1.Id, CreatedAt = DateTime.UtcNow.AddHours(-12) }
                            }
                        },
                        new EduCore.Domain.Entities.ForumModel.ForumPost
                        {
                            LessonId = algebraLesson.Id,
                            StudentId = student2.Id,
                            Title = "Great lesson!",
                            Body = "The explanation was very clear, thank you.",
                            CreatedAt = DateTime.UtcNow.AddDays(-1),
                            UpvoteCount = 0
                        }
                    };

                    if (biologyLesson != null)
                    {
                        posts.Add(new EduCore.Domain.Entities.ForumModel.ForumPost
                        {
                            LessonId = biologyLesson.Id,
                            StudentId = student1.Id,
                            Title = "Question about cell membranes",
                            Body = "What does semi-permeable actually mean in this context?",
                            CreatedAt = DateTime.UtcNow.AddHours(-5),
                            UpvoteCount = 1,
                            Upvotes = new List<EduCore.Domain.Entities.ForumModel.PostUpvote>
                            {
                                new EduCore.Domain.Entities.ForumModel.PostUpvote { UserId = student2.Id, CreatedAt = DateTime.UtcNow.AddHours(-2) }
                            }
                        });
                    }

                    context.Set<EduCore.Domain.Entities.ForumModel.ForumPost>().AddRange(posts);
                    await context.SaveChangesAsync();
                }
            }
        }

        ///Helper 
        private static async Task SeedUserAsync(
            UserManager<User> userManager,
            User user,
            string password,
            string role)
        {
            if (await userManager.FindByEmailAsync(user.Email!) is null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
