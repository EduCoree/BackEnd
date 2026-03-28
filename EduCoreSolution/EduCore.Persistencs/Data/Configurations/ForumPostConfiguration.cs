using EduCore.Domain.Entities.ForumModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class ForumPostConfiguration : IEntityTypeConfiguration<ForumPost>
    {
        public void Configure(EntityTypeBuilder<ForumPost> builder)
        {
            builder.ToTable("forum_posts");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Body).IsRequired().HasColumnType("NVARCHAR(MAX)");
            builder.Property(p => p.UpvoteCount).HasDefaultValue(0);
            builder.Property(p => p.IsRemoved).HasDefaultValue(false);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(p => p.Course)
                   .WithMany(c => c.ForumPosts)
                   .HasForeignKey(p => p.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Student)
                   .WithMany(u => u.ForumPosts)
                   .HasForeignKey(p => p.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
