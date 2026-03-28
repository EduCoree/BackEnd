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
    public class VideoLessonConfiguration : IEntityTypeConfiguration<VideoLesson>
    {
        public void Configure(EntityTypeBuilder<VideoLesson> builder)
        {
            builder.ToTable("video_lessons");
            builder.HasKey(v => v.Id);
            builder.HasIndex(v => v.LessonId).IsUnique(); // 1-to-1

            builder.Property(v => v.VideoUrl).IsRequired().HasMaxLength(255);
            builder.Property(v => v.VideoProvider).HasMaxLength(40);
            builder.Property(v => v.ThumbnailUrl).HasMaxLength(255);

            builder.HasOne(v => v.Lesson)
                   .WithOne(l => l.VideoLesson)
                   .HasForeignKey<VideoLesson>(v => v.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
