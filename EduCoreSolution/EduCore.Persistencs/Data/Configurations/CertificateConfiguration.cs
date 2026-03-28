using EduCore.Domain.Entities.ProgressModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.ToTable("certificates");
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => new { c.StudentId, c.CourseId }).IsUnique();

            builder.Property(c => c.CertificateUrl).HasMaxLength(255);
            builder.Property(c => c.IssuedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(c => c.Student)
                   .WithMany(u => u.Certificates)
                   .HasForeignKey(c => c.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Course)
                   .WithMany(co => co.Certificates)
                   .HasForeignKey(c => c.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
