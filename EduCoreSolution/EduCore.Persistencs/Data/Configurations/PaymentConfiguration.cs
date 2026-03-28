using EduCore.Domain.Entities.EnrollmentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount).HasColumnType("DECIMAL(10,2)").IsRequired();
            builder.Property(p => p.Currency).HasMaxLength(3).HasDefaultValue("USD");
            builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(30);
            builder.Property(p => p.Status).HasConversion<string>()
                   .HasMaxLength(20).HasDefaultValue(PaymentStatus.Pending);
            builder.Property(p => p.Reference).HasMaxLength(100);

            builder.HasOne(p => p.Enrollment)
                   .WithOne(e => e.Payment)
                   .HasForeignKey<Payment>(p => p.EnrollmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Student)
                   .WithMany(u => u.Payments)
                   .HasForeignKey(p => p.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
