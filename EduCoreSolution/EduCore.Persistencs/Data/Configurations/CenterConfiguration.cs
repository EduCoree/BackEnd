using EduCore.Domain.Entities.CenterModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class CenterConfiguration : IEntityTypeConfiguration<Center>
    {
        public void Configure(EntityTypeBuilder<Center> builder)
        {
            builder.ToTable("centers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(120);

            builder.Property(c => c.LogoUrl)
                   .HasMaxLength(255);

            builder.Property(c => c.ContactEmail)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(c => c.Phone)
                   .HasMaxLength(20);

            builder.Property(c => c.Address)
                   .HasColumnType("NVARCHAR(MAX)");

            builder.Property(c => c.SocialLinks)
                   .HasColumnType("NVARCHAR(MAX)");  

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Relations
            builder.HasMany(c => c.Users)
                   .WithOne(u => u.Center)
                   .HasForeignKey(u => u.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Categories)
                   .WithOne(cat => cat.Center)
                   .HasForeignKey(cat => cat.CenterId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
