using EduCore.Domain.Entities.ContentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class LiveSessionConfiguration : IEntityTypeConfiguration<LiveSession>
    {
        public void Configure(EntityTypeBuilder<LiveSession> builder)
        {
            builder.ToTable("live_sessions");
            builder.HasKey(ls => ls.Id);
            builder.HasIndex(ls => ls.LessonId).IsUnique();

            builder.Property(ls => ls.Provider).HasConversion<string>().HasMaxLength(30);
            builder.Property(ls => ls.MeetingUrl).IsRequired().HasMaxLength(255);
            builder.Property(ls => ls.RecordingUrl).HasMaxLength(255);

            builder.HasOne(ls => ls.Lesson)
                   .WithOne(l => l.LiveSession)
                   .HasForeignKey<LiveSession>(ls => ls.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(ls => ls.AttendanceRecords)
                   .WithOne(ar => ar.LiveSession)
                   .HasForeignKey(ar => ar.LiveSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
