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
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("questions");
            builder.HasKey(q => q.Id);

            builder.Property(q => q.Text).IsRequired().HasColumnType("NVARCHAR(MAX)");
            builder.Property(q => q.Points).HasDefaultValue(1);

            builder.Property(q => q.Type)
                   .HasConversion<string>()
                   .HasMaxLength(20);     // "MCQ" or "TrueFalse"

            builder.HasOne(q => q.Quiz)
                   .WithMany(qz => qz.Questions)
                   .HasForeignKey(q => q.QuizId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
