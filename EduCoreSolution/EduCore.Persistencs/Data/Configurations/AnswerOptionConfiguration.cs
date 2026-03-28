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
    public class AnswerOptionConfiguration : IEntityTypeConfiguration<AnswerOption>
    {
        public void Configure(EntityTypeBuilder<AnswerOption> builder)
        {
            builder.ToTable("answer_options");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Text).IsRequired().HasColumnType("NVARCHAR(MAX)");
            builder.Property(a => a.IsCorrect).HasDefaultValue(false);

            builder.HasOne(a => a.Question)
                   .WithMany(q => q.AnswerOptions)
                   .HasForeignKey(a => a.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
