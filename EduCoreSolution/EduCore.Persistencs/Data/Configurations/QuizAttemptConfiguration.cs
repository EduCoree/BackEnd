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
    public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
    {
        public void Configure(EntityTypeBuilder<QuizAttempt> builder)
        {
            builder.ToTable("quiz_attempts");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Score).HasColumnType("DECIMAL(5,2)");
            builder.Property(a => a.StartedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(a => a.Passed).HasDefaultValue(false);

            builder.HasOne(a => a.Student)
                   .WithMany(u => u.QuizAttempts)
                   .HasForeignKey(a => a.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Quiz)
                   .WithMany(q => q.Attempts)
                   .HasForeignKey(a => a.QuizId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
