using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
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
    public class CourseReviewConfiguration : IEntityTypeConfiguration<CourseReview>
    {
        public void Configure(EntityTypeBuilder<CourseReview> builder)
        {
            builder.ToTable("course_reviews");
            builder.HasKey(r => r.Id);
            builder.HasIndex(r => new { r.StudentId, r.CourseId }).IsUnique();

            builder.Property(r => r.Rating)
                   .HasColumnType("TINYINT")   
                   .IsRequired();

            // Rating must be between 1 and 5
            builder.ToTable("course_reviews", t =>
                t.HasCheckConstraint("CK_reviews_rating", "rating BETWEEN 1 AND 5"));

            builder.Property(r => r.Comment).HasColumnType("NVARCHAR(MAX)");
            builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(r => r.Student)
                   .WithMany(u => u.CourseReviews)
                   .HasForeignKey(r => r.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Course)
                   .WithMany(c => c.Reviews)
                   .HasForeignKey(r => r.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
