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
    public class ForumReplyConfiguration : IEntityTypeConfiguration<ForumReply>
    {
        public void Configure(EntityTypeBuilder<ForumReply> builder)
        {
            builder.ToTable("forum_replies");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Body).IsRequired().HasColumnType("NVARCHAR(MAX)");
            builder.Property(r => r.UpvoteCount).HasDefaultValue(0);
            builder.Property(r => r.IsRemoved).HasDefaultValue(false);
            builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(r => r.Post)
                   .WithMany(p => p.Replies)
                   .HasForeignKey(r => r.PostId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.User)
                   .WithMany(u => u.ForumReplies)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
