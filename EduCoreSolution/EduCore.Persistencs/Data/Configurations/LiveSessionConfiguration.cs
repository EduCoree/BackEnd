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
            
            // LessonId is now optional (0 or 1 per lesson, but not required)
            builder.HasIndex(ls => ls.LessonId).IsUnique().HasFilter("[LessonId] IS NOT NULL");

            builder.Property(ls => ls.Provider).HasConversion<string>().HasMaxLength(30);
            builder.Property(ls => ls.MeetingUrl).IsRequired().HasMaxLength(255);
            builder.Property(ls => ls.RecordingUrl).HasMaxLength(255);
            builder.Property(ls => ls.Title).HasMaxLength(200);
            builder.Property(ls => ls.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(ls => ls.Course)
                   .WithMany(c => c.LiveSessions)
                   .HasForeignKey(ls => ls.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ls => ls.Lesson)
                   .WithOne(l => l.LiveSession)
                   .HasForeignKey<LiveSession>(ls => ls.LessonId)
                   .OnDelete(DeleteBehavior.ClientSetNull); // if lesson is deleted, don't necessarily delete the session, or cascade is fine too. Using SetNull because it's optional.

            builder.HasMany(ls => ls.AttendanceRecords)
                   .WithOne(ar => ar.LiveSession)
                   .HasForeignKey(ar => ar.LiveSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
