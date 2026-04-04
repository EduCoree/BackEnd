using EduCore.Domain.Entities.ContentModel;
using EduCore.Domain.Entities.CourseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.ToTable("lessons");
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Title).IsRequired().HasMaxLength(160);
            builder.Property(l => l.SortOrder).HasDefaultValue(0);
            builder.Property(l => l.IsFreePreview).HasDefaultValue(false);

            // store [Flags] enum as INT  (Video=1, Pdf=2, Live=4, Video+Pdf=3 …)
            builder.Property(l => l.Type)
                   .HasColumnName("type")
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(l => l.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(l => l.DeletedAt).IsRequired(false);

            // Global Query Filter for soft-delete
            builder.HasQueryFilter(l => l.DeletedAt == null);

            builder.HasOne(l => l.Section)
                   .WithMany(s => s.Lessons)
                   .HasForeignKey(l => l.SectionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1-to-1 content children
            builder.HasOne(l => l.VideoLesson)
                   .WithOne(v => v.Lesson)
                   .HasForeignKey<VideoLesson>(v => v.LessonId);

            builder.HasOne(l => l.PdfLesson)
                   .WithOne(p => p.Lesson)
                   .HasForeignKey<PdfLesson>(p => p.LessonId);

            builder.HasOne(l => l.LiveSession)
                   .WithOne(ls => ls.Lesson)
                   .HasForeignKey<LiveSession>(ls => ls.LessonId);
        }

    }
}
