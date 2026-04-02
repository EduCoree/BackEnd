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
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("enrollments");
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.Status).HasConversion<string>()
                   .HasMaxLength(20).HasDefaultValue(EnrollmentStatus.Active);
            builder.Property(e => e.EnrolledAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(e => e.Student)
                   .WithMany(u => u.Enrollments)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Payment)
                   .WithOne(p => p.Enrollment)
                   .HasForeignKey<Payment>(p => p.EnrollmentId);
        }
    }
}
