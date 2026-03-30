using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.ContentModel;
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
    public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
    {
        public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
        {
            builder.ToTable("attendance_records");
            builder.HasKey(a => a.Id);
            builder.HasIndex(a => new { a.StudentId, a.LiveSessionId }).IsUnique();

            builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(a => a.JoinedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(a => a.Student)
                   .WithMany(u => u.AttendanceRecords)
                   .HasForeignKey(a => a.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.LiveSession)
                   .WithMany(ls => ls.AttendanceRecords)
                   .HasForeignKey(a => a.LiveSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
