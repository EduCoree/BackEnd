using EduCore.Domain.Entities.QuizModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.ToTable("quizzes");
            builder.HasKey(q => q.Id);

            builder.Property(q => q.Title).IsRequired().HasMaxLength(160);
            builder.Property(q => q.PassScore).HasDefaultValue(60);
            builder.Property(q => q.MaxAttempts).HasDefaultValue(1);
            builder.Property(q => q.IsRandomized).HasDefaultValue(false);

            builder.HasOne(q => q.Course)
                   .WithMany(c => c.Quizzes)
                   .HasForeignKey(q => q.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
