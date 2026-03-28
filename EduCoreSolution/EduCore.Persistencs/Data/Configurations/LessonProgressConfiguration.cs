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
    public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
    {
        public void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            builder.ToTable("lesson_progress");
            builder.HasKey(lp => lp.Id);
            builder.HasIndex(lp => new { lp.StudentId, lp.LessonId }).IsUnique();

            builder.Property(lp => lp.IsCompleted).HasDefaultValue(false);

            builder.HasOne(lp => lp.Student)
                   .WithMany(u => u.LessonProgresses)
                   .HasForeignKey(lp => lp.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lp => lp.Lesson)
                   .WithMany(l => l.Progresses)
                   .HasForeignKey(lp => lp.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
