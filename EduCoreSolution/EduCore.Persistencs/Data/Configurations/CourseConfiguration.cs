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
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("courses");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title).IsRequired().HasMaxLength(160);
            builder.Property(c => c.Description).HasColumnType("NVARCHAR(MAX)");
            builder.Property(c => c.CoverImage).HasMaxLength(255);
            builder.Property(c => c.Price).HasColumnType("DECIMAL(10,2)");
            builder.Property(c => c.DiscountedPrice).HasColumnType("DECIMAL(10,2)");

            builder.Property(c => c.Level).HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.PricingType).HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.Status).HasConversion<string>()
                   .HasMaxLength(20).HasDefaultValue(CourseStatus.Draft);

            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            // FK → categories
            builder.HasOne(c => c.Category)
                   .WithMany(cat => cat.Courses)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK → users (teacher)
            builder.HasOne(c => c.Teacher)
                   .WithMany(u => u.TaughtCourses)
                   .HasForeignKey(c => c.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
