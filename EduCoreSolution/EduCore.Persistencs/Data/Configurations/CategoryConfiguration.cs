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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.Slug).IsUnique();

            builder.Property(c => c.Name).IsRequired().HasMaxLength(80);
            builder.Property(c => c.Slug).IsRequired().HasMaxLength(80);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(c => c.Center)
                   .WithMany(ce => ce.Categories)
                   .HasForeignKey(c => c.CenterId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Courses)
                   .WithOne(co => co.Category)
                   .HasForeignKey(co => co.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
