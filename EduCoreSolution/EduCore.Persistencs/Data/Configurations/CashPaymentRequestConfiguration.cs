using EduCore.Domain.Entities.EnrollmentModel;
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
    public class CashPaymentRequestConfiguration : IEntityTypeConfiguration<CashPaymentRequest>
    {
        public void Configure(EntityTypeBuilder<CashPaymentRequest> builder)
        {
            builder.ToTable("cash_payment_requests");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.StudentId)
                   .IsRequired();

            builder.Property(e => e.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .HasDefaultValue(CashRequestStatus.Pending);

            builder.Property(e => e.RequestedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(e => e.Student)
                   .WithMany()
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Course)
                   .WithMany()
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
