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
    public class AttemptAnswerConfiguration : IEntityTypeConfiguration<AttemptAnswer>
    {
        public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
        {
            builder.ToTable("attempt_answers");
            builder.HasKey(a => a.Id);

            // Prevent duplicate answers per question per attempt
            builder.HasIndex(a => new { a.AttemptId, a.QuestionId }).IsUnique();

            builder.HasOne(a => a.Attempt)
                   .WithMany(qa => qa.AttemptAnswers)
                   .HasForeignKey(a => a.AttemptId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Question)
                   .WithMany(q => q.AttemptAnswers)
                   .HasForeignKey(a => a.QuestionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.AnswerOption)
                   .WithMany(ao => ao.AttemptAnswers)
                   .HasForeignKey(a => a.AnswerOptionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
