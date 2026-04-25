using EduCore.Domain.Entities.PayoutModel;
using EduCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class TeacherEarningConfiguration : IEntityTypeConfiguration<TeacherEarning>
    {
        public void Configure(EntityTypeBuilder<TeacherEarning> builder)
        {
            builder.ToTable("teacher_earnings");
            builder.HasKey(e => e.Id);

            // Financial columns - matching Payment precision
            builder.Property(e => e.GrossAmount).HasColumnType("DECIMAL(10,2)").IsRequired();
            builder.Property(e => e.CommissionRate).HasColumnType("DECIMAL(5,4)").IsRequired(); // e.g. 0.8000
            builder.Property(e => e.NetAmount).HasColumnType("DECIMAL(10,2)").IsRequired();
            builder.Property(e => e.PlatformFee).HasColumnType("DECIMAL(10,2)").IsRequired();
            builder.Property(e => e.Currency).HasMaxLength(3).HasDefaultValue("EGP");

            builder.Property(e => e.EarnedAt).HasDefaultValueSql("GETUTCDATE()");

            // Store as string but WITHOUT a database-generated default.
            // The C# default (`EarningStatus.Available` set on the entity) is the source of truth
            // and gets written on every insert. This avoids the "sentinel value" EF warning.
            builder.Property(e => e.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            // FK → users (teacher)
            builder.HasOne(e => e.Teacher)
                   .WithMany(u => u.Earnings)
                   .HasForeignKey(e => e.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → courses
            builder.HasOne(e => e.Course)
                   .WithMany()
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → payments (one payment produces at most one earning)
            builder.HasOne(e => e.Payment)
                   .WithOne()
                   .HasForeignKey<TeacherEarning>(e => e.PaymentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → enrollments
            builder.HasOne(e => e.Enrollment)
                   .WithOne()
                   .HasForeignKey<TeacherEarning>(e => e.EnrollmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → invoices (nullable: set when earning is added to an invoice)
            builder.HasOne(e => e.Invoice)
                   .WithMany(i => i.Earnings)
                   .HasForeignKey(e => e.InvoiceId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes for fast lookups during invoice generation
            builder.HasIndex(e => new { e.TeacherId, e.EarnedAt });
            builder.HasIndex(e => new { e.TeacherId, e.Status });
            builder.HasIndex(e => e.InvoiceId);
        }
    }
}
