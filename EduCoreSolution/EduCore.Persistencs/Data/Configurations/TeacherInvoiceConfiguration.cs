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
    public class TeacherInvoiceConfiguration : IEntityTypeConfiguration<TeacherInvoice>
    {
        public void Configure(EntityTypeBuilder<TeacherInvoice> builder)
        {
            builder.ToTable("teacher_invoices");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(30);
            builder.HasIndex(i => i.InvoiceNumber).IsUnique();

            builder.Property(i => i.EarningsTotal).HasColumnType("DECIMAL(12,2)").IsRequired();
            builder.Property(i => i.TierBonus).HasColumnType("DECIMAL(10,2)").HasDefaultValue(0m);
            builder.Property(i => i.TotalAmount).HasColumnType("DECIMAL(12,2)").IsRequired();
            builder.Property(i => i.Currency).HasMaxLength(3).HasDefaultValue("EGP");

            builder.Property(i => i.Status).HasConversion<string>()
                   .HasMaxLength(20).HasDefaultValue(InvoiceStatus.Draft);

            builder.Property(i => i.PayoutMethod).HasConversion<string>().HasMaxLength(30);
            builder.Property(i => i.PayoutReference).HasMaxLength(100);
            builder.Property(i => i.Notes).HasMaxLength(500);

            builder.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            // FK → users (teacher)
            builder.HasOne(i => i.Teacher)
                   .WithMany(u => u.Invoices)
                   .HasForeignKey(i => i.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            // One teacher can have only one invoice per period (prevents duplicates from re-running the job)
            builder.HasIndex(i => new { i.TeacherId, i.PeriodStart, i.PeriodEnd }).IsUnique();

            builder.HasIndex(i => i.Status);
        }
    }
}
