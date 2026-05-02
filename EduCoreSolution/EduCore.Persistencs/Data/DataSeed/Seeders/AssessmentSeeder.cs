using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestionType = EduCore.Shared.Enums.QuestionType;

namespace EduCore.Persistencs.Data.DataSeed.Seeders
{
    public static class AssessmentSeeder
    {
        public static async Task SeedQuizzesAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.Quizzes.AnyAsync()) return;

            var algebra = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Algebra for Beginners");
            var biology = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Advanced Biology");
            var nahw = await context.Courses.FirstOrDefaultAsync(c => c.Title == "أساسيات النحو العربي");
            if (algebra == null || biology == null) return;

            var quizzes = new List<Quiz>
            {
                // English quiz
                new Quiz
                {
                    CourseId = algebra.Id,
                    Title = "Algebra Basics Quiz",
                    TimeLimitMins = 20, PassScore = 70, MaxAttempts = 3,
                    IsRandomized = false, IsPublished = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Text = "What is the value of x in 2x = 10?",
                            Type = QuestionType.MCQ, Points = 10,
                            AnswerOptions = new List<AnswerOption>
                            {
                                new AnswerOption { Text = "3", IsCorrect = false },
                                new AnswerOption { Text = "5", IsCorrect = true  },
                                new AnswerOption { Text = "10", IsCorrect = false },
                                new AnswerOption { Text = "20", IsCorrect = false }
                            }
                        },
                        new Question
                        {
                            Text = "A variable can hold different values.",
                            Type = QuestionType.TrueFalse, Points = 5,
                            AnswerOptions = new List<AnswerOption>
                            {
                                new AnswerOption { Text = "True",  IsCorrect = true },
                                new AnswerOption { Text = "False", IsCorrect = false }
                            }
                        },
                        new Question
                        {
                            Text = "Which is a linear equation?",
                            Type = QuestionType.MCQ, Points = 10,
                            AnswerOptions = new List<AnswerOption>
                            {
                                new AnswerOption { Text = "y = 2x + 3", IsCorrect = true  },
                                new AnswerOption { Text = "y = x²",     IsCorrect = false },
                                new AnswerOption { Text = "y = 1/x",    IsCorrect = false },
                                new AnswerOption { Text = "y = √x",     IsCorrect = false }
                            }
                        }
                    }
                },
                // English quiz
                new Quiz
                {
                    CourseId = biology.Id,
                    Title = "Cell Biology Quiz",
                    TimeLimitMins = 30, PassScore = 60, MaxAttempts = 2,
                    IsRandomized = true, IsPublished = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Text = "What is the powerhouse of the cell?",
                            Type = QuestionType.MCQ, Points = 10,
                            AnswerOptions = new List<AnswerOption>
                            {
                                new AnswerOption { Text = "Nucleus",      IsCorrect = false },
                                new AnswerOption { Text = "Mitochondria", IsCorrect = true  },
                                new AnswerOption { Text = "Ribosome",     IsCorrect = false },
                                new AnswerOption { Text = "Golgi",        IsCorrect = false }
                            }
                        },
                        new Question
                        {
                            Text = "DNA is found in the nucleus.",
                            Type = QuestionType.TrueFalse, Points = 5,
                            AnswerOptions = new List<AnswerOption>
                            {
                                new AnswerOption { Text = "True",  IsCorrect = true },
                                new AnswerOption { Text = "False", IsCorrect = false }
                            }
                        }
                    }
                }
            };

            // Arabic quiz (only if the Arabic course exists)
            if (nahw != null)
            {
                quizzes.Add(new Quiz
                {
                    CourseId = nahw.Id,
                    Title = "اختبار النحو العربي",
                    TimeLimitMins = 25,
                    PassScore = 60,
                    MaxAttempts = 3,
                    IsRandomized = false,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Text = "ما هو إعراب كلمة \"محمد\" في الجملة: محمدٌ مجتهدٌ؟",
                            Type = QuestionType.MCQ, Points = 10,
                            AnswerOptions = new List<AnswerOption>
                            {
                                new AnswerOption { Text = "مبتدأ مرفوع", IsCorrect = true  },
                                new AnswerOption { Text = "خبر مرفوع",   IsCorrect = false },
                                new AnswerOption { Text = "فاعل مرفوع",  IsCorrect = false },
                                new AnswerOption { Text = "مفعول به",    IsCorrect = false }
                            }
                        },
                        new Question
                        {
                            Text = "الفاعل دائماً مرفوع.",
                            Type = QuestionType.TrueFalse, Points = 5,
                            AnswerOptions = new List<AnswerOption>
                            {
                                new AnswerOption { Text = "صح",  IsCorrect = true  },
                                new AnswerOption { Text = "خطأ", IsCorrect = false }
                            }
                        },
                        new Question
                        {
                            Text = "أيُّ الكلمات التالية فعل ماضٍ؟",
                            Type = QuestionType.MCQ, Points = 10,
                            AnswerOptions = new List<AnswerOption>
                            {
                                new AnswerOption { Text = "كَتَبَ",   IsCorrect = true  },
                                new AnswerOption { Text = "يكتبُ",   IsCorrect = false },
                                new AnswerOption { Text = "اكتبْ",   IsCorrect = false },
                                new AnswerOption { Text = "كتابة",   IsCorrect = false }
                            }
                        }
                    }
                });
            }

            context.Quizzes.AddRange(quizzes);
            await context.SaveChangesAsync();
            logger.LogInformation("Quizzes seeded ({Count}) with questions and options.", quizzes.Count);
        }

        public static async Task SeedQuizAttemptsAsync(
            EduCoreDbContext context, UserManager<User> userManager, ILogger logger)
        {
            if (await context.QuizAttempts.AnyAsync()) return;

            var s1 = await userManager.FindByEmailAsync("ali.mahmoud@educore.com");
            var s2 = await userManager.FindByEmailAsync("nour.hassan@educore.com");
            if (s1 == null || s2 == null) return;

            var quiz = await context.Quizzes
                .Include(q => q.Questions).ThenInclude(qu => qu.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Title == "Algebra Basics Quiz");
            if (quiz == null || !quiz.Questions.Any()) return;

            // Student1: passed (picks correct option)
            var passedAttempt = new QuizAttempt
            {
                StudentId = s1.Id,
                QuizId = quiz.Id,
                Score = 25,
                StartedAt = DateTime.UtcNow.AddDays(-3),
                SubmittedAt = DateTime.UtcNow.AddDays(-3).AddMinutes(15),
                Passed = true,
                AttemptAnswers = quiz.Questions.Select(q => new AttemptAnswer
                {
                    QuestionId = q.Id,
                    AnswerOptionId = q.AnswerOptions.First(o => o.IsCorrect).Id
                }).ToList()
            };

            // Student2: failed (picks first option)
            var failedAttempt = new QuizAttempt
            {
                StudentId = s2.Id,
                QuizId = quiz.Id,
                Score = 5,
                StartedAt = DateTime.UtcNow.AddDays(-1),
                SubmittedAt = DateTime.UtcNow.AddDays(-1).AddMinutes(20),
                Passed = false,
                AttemptAnswers = quiz.Questions.Select(q => new AttemptAnswer
                {
                    QuestionId = q.Id,
                    AnswerOptionId = q.AnswerOptions.First().Id
                }).ToList()
            };

            context.QuizAttempts.AddRange(passedAttempt, failedAttempt);
            await context.SaveChangesAsync();
            logger.LogInformation("QuizAttempts seeded (2) with answers.");
        }
    }
}