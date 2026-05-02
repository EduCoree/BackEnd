using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.PayoutModel;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduCore.Persistencs.Data.DataSeed.Seeders
{
    public static class CommerceSeeder
    {
        public static async Task SeedEnrollmentsAsync(
            EduCoreDbContext context, UserManager<User> userManager, ILogger logger)
        {
            if (await context.Enrollments.AnyAsync()) return;
            var s1 = await userManager.FindByEmailAsync("ali.mahmoud@educore.com");
            var s2 = await userManager.FindByEmailAsync("nour.hassan@educore.com");
            var s3 = await userManager.FindByEmailAsync("youssef.ibrahim@educore.com");
            var s4 = await userManager.FindByEmailAsync("layla.abdullah@educore.com");
            if (s1 == null || s2 == null || s3 == null || s4 == null) return;

            var algebra = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Algebra for Beginners");
            var calculus = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Calculus Fundamentals");
            var biology = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Advanced Biology");
            var arabic = await context.Courses.FirstOrDefaultAsync(c => c.Title == "أساسيات النحو العربي");
            var english = await context.Courses.FirstOrDefaultAsync(c => c.Title == "English Conversation B2");

            var enrollments = new List<Enrollment>();

            if (algebra != null)
                enrollments.Add(new Enrollment { StudentId = s1.Id, CourseId = algebra.Id, Type = EnrollmentType.Paid, EnrolledAt = DateTime.UtcNow.AddDays(-25), Status = EnrollmentStatus.Active });
            if (calculus != null)
                enrollments.Add(new Enrollment { StudentId = s1.Id, CourseId = calculus.Id, Type = EnrollmentType.Free, EnrolledAt = DateTime.UtcNow.AddDays(-15), Status = EnrollmentStatus.Active });
            if (biology != null)
                enrollments.Add(new Enrollment { StudentId = s2.Id, CourseId = biology.Id, Type = EnrollmentType.Paid, EnrolledAt = DateTime.UtcNow.AddDays(-10), Status = EnrollmentStatus.Active });
            if (algebra != null)
                enrollments.Add(new Enrollment { StudentId = s2.Id, CourseId = algebra.Id, Type = EnrollmentType.Paid, EnrolledAt = DateTime.UtcNow.AddDays(-20), Status = EnrollmentStatus.Completed });
            if (arabic != null)
                enrollments.Add(new Enrollment { StudentId = s3.Id, CourseId = arabic.Id, Type = EnrollmentType.Paid, EnrolledAt = DateTime.UtcNow.AddDays(-8), Status = EnrollmentStatus.Active, ExpiresAt = DateTime.UtcNow.AddDays(22) });
            if (english != null)
                enrollments.Add(new Enrollment { StudentId = s4.Id, CourseId = english.Id, Type = EnrollmentType.Paid, EnrolledAt = DateTime.UtcNow.AddDays(-3), Status = EnrollmentStatus.Active });

            if (enrollments.Any())
            {
                context.Enrollments.AddRange(enrollments);
                await context.SaveChangesAsync();
                logger.LogInformation("Enrollments seeded ({Count}).", enrollments.Count);
            }
        }

        public static async Task SeedPaymentsAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.Payments.AnyAsync()) return;

            // Create one Payment per Paid enrollment
            var paidEnrollments = await context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.Type == EnrollmentType.Paid)
                .ToListAsync();

            var payments = new List<Payment>();
            int i = 0;
            foreach (var en in paidEnrollments)
            {
                payments.Add(new Payment
                {
                    EnrollmentId = en.Id,
                    StudentId = en.StudentId,
                    Amount = en.Course.Price,
                    Currency = "EGP",
                    Method = (i % 3) switch { 0 => PaymentMethod.Paymob, 1 => PaymentMethod.CreditCard, _ => PaymentMethod.Fawry },
                    Status = PaymentStatus.Completed,
                    Reference = $"PAY-{Guid.NewGuid().ToString("N")[..10].ToUpper()}",
                    PaidAt = en.EnrolledAt
                });
                i++;
            }

            if (payments.Any())
            {
                context.Payments.AddRange(payments);
                await context.SaveChangesAsync();
                logger.LogInformation("Payments seeded ({Count}).", payments.Count);
            }
        }

        public static async Task SeedCashPaymentRequestsAsync(
            EduCoreDbContext context, UserManager<User> userManager, ILogger logger)
        {
            if (await context.CashPaymentRequests.AnyAsync()) return;

            var s5 = await userManager.FindByEmailAsync("karim.khaled@educore.com");
            var s3 = await userManager.FindByEmailAsync("youssef.ibrahim@educore.com");
            var biology = await context.Courses.FirstOrDefaultAsync(c => c.Title == "Advanced Biology");
            var english = await context.Courses.FirstOrDefaultAsync(c => c.Title == "English Conversation B2");
            if (s5 == null || biology == null) return;

            var requests = new List<CashPaymentRequest>
            {
                new CashPaymentRequest
                {
                    StudentId = s5.Id, CourseId = biology.Id,
                    Status = CashRequestStatus.Pending,
                    RequestedAt = DateTime.UtcNow.AddHours(-6)
                }
            };

            if (s3 != null && english != null)
            {
                requests.Add(new CashPaymentRequest
                {
                    StudentId = s3.Id,
                    CourseId = english.Id,
                    Status = CashRequestStatus.Confirmed,
                    RequestedAt = DateTime.UtcNow.AddDays(-2)
                });
            }

            context.CashPaymentRequests.AddRange(requests);
            await context.SaveChangesAsync();
            logger.LogInformation("CashPaymentRequests seeded ({Count}).", requests.Count);
        }

        public static async Task SeedTeacherEarningsAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.TeacherEarnings.AnyAsync()) return;

            var settings = await context.PayoutSettings.FirstOrDefaultAsync();
            if (settings == null) return;

            // One earning per completed payment
            var payments = await context.Payments
                .Include(p => p.Enrollment).ThenInclude(e => e!.Course)
                .Where(p => p.Status == PaymentStatus.Completed && p.EnrollmentId != null)
                .ToListAsync();

            var earnings = new List<TeacherEarning>();
            foreach (var p in payments)
            {
                if (p.Enrollment == null) continue;

                var net = p.Amount * settings.TeacherCommissionRate;
                earnings.Add(new TeacherEarning
                {
                    TeacherId = p.Enrollment.Course.TeacherId,
                    CourseId = p.Enrollment.CourseId,
                    PaymentId = p.Id,
                    EnrollmentId = p.Enrollment.Id,
                    GrossAmount = p.Amount,
                    CommissionRate = settings.TeacherCommissionRate,
                    NetAmount = net,
                    PlatformFee = p.Amount - net,
                    Currency = "EGP",
                    EarnedAt = p.PaidAt ?? DateTime.UtcNow,
                    Status = EarningStatus.Available
                });
            }

            if (earnings.Any())
            {
                context.TeacherEarnings.AddRange(earnings);
                await context.SaveChangesAsync();
                logger.LogInformation("TeacherEarnings seeded ({Count}).", earnings.Count);
            }
        }

        public static async Task SeedTeacherInvoicesAsync(EduCoreDbContext context, ILogger logger)
        {
            if (await context.TeacherInvoices.AnyAsync()) return;

            var settings = await context.PayoutSettings.FirstOrDefaultAsync();
            if (settings == null) return;

            // Group available earnings by teacher (last month)
            var lastMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-1);
            var lastMonthEnd = lastMonthStart.AddMonths(1).AddSeconds(-1);

            var groups = await context.TeacherEarnings
                .Where(e => e.Status == EarningStatus.Available)
                .GroupBy(e => e.TeacherId)
                .Select(g => new
                {
                    TeacherId = g.Key,
                    Total = g.Sum(x => x.NetAmount),
                    Count = g.Count(),
                    Ids = g.Select(x => x.Id).ToList()
                })
                .ToListAsync();

            int seq = 1;
            foreach (var g in groups)
            {
                var bonus = settings.CalculateTierBonus(g.Count);
                var invoice = new TeacherInvoice
                {
                    InvoiceNumber = $"INV-{lastMonthStart:yyyy-MM}-{seq:D3}",
                    TeacherId = g.TeacherId,
                    PeriodStart = lastMonthStart,
                    PeriodEnd = lastMonthEnd,
                    PaidEnrollmentsCount = g.Count,
                    EarningsTotal = g.Total,
                    TierBonus = bonus,
                    TotalAmount = g.Total + bonus,
                    Currency = "EGP",
                    Status = InvoiceStatus.Issued,
                    CreatedAt = DateTime.UtcNow,
                    IssuedAt = DateTime.UtcNow
                };
                context.TeacherInvoices.Add(invoice);
                await context.SaveChangesAsync();

                // Link earnings to this invoice and mark as Invoiced
                var ids = g.Ids;
                var earnings = await context.TeacherEarnings.Where(e => ids.Contains(e.Id)).ToListAsync();
                foreach (var e in earnings)
                {
                    e.InvoiceId = invoice.Id;
                    e.Status = EarningStatus.Invoiced;
                }
                await context.SaveChangesAsync();
                seq++;
            }

            if (groups.Any())
                logger.LogInformation("TeacherInvoices seeded ({Count}).", groups.Count);
        }
    }
}