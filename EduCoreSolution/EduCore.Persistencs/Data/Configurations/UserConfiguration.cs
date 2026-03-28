using EduCore.Domain.Entities.AuthModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("AspNetUsers");  // Identity default table name

            

            builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
            builder.Property(u => u.AvatarUrl).HasMaxLength(255);
            builder.Property(u => u.Bio).HasColumnType("NVARCHAR(MAX)");

            builder.Property(u => u.Role)
                   .HasConversion<string>()       // stored as "Student"/"Teacher"/"Admin"
                   .HasMaxLength(20);

            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(u => u.Center)
                   .WithMany(c => c.Users)
                   .HasForeignKey(u => u.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
