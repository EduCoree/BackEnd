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
    public class PdfLessonConfiguration : IEntityTypeConfiguration<PdfLesson>
    {
        public void Configure(EntityTypeBuilder<PdfLesson> builder)
        {
            builder.ToTable("pdf_lessons");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.LessonId).IsUnique();

            builder.Property(p => p.FileUrl).IsRequired().HasMaxLength(255);

            builder.HasOne(p => p.Lesson)
                   .WithOne(l => l.PdfLesson)
                   .HasForeignKey<PdfLesson>(p => p.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
