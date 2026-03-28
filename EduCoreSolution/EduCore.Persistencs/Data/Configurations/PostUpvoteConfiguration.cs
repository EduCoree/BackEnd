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
    public class PostUpvoteConfiguration : IEntityTypeConfiguration<PostUpvote>
    {
        public void Configure(EntityTypeBuilder<PostUpvote> builder)
        {
            builder.ToTable("post_upvotes");
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => new { u.UserId, u.PostId }).IsUnique(); // one upvote per user

            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(u => u.User)
                   .WithMany(usr => usr.PostUpvotes)
                   .HasForeignKey(u => u.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Post)
                   .WithMany(p => p.Upvotes)
                   .HasForeignKey(u => u.PostId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
