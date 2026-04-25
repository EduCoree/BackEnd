using EduCore.Domain.Entities.PayoutModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class PayoutSettingsConfiguration : IEntityTypeConfiguration<PayoutSettings>
    {
        public void Configure(EntityTypeBuilder<PayoutSettings> builder)
        {
            builder.ToTable("payout_settings");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.TeacherCommissionRate)
                   .HasColumnType("DECIMAL(5,4)").IsRequired();

            builder.Property(s => s.Tier1Bonus).HasColumnType("DECIMAL(10,2)").IsRequired();
            builder.Property(s => s.Tier2Bonus).HasColumnType("DECIMAL(10,2)").IsRequired();
            builder.Property(s => s.Tier3Bonus).HasColumnType("DECIMAL(10,2)").IsRequired();

            builder.Property(s => s.Currency).HasMaxLength(3).HasDefaultValue("EGP");
            builder.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(s => s.UpdatedBy).HasMaxLength(450); // IdentityUser.Id length

            // Seed the single config row so the system works out of the box.
            // Admins can later update these values via the admin API.
            builder.HasData(new PayoutSettings
            {
                Id = 1,
                TeacherCommissionRate = 0.80m,
                Tier1Threshold = 10,
                Tier1Bonus = 500m,
                Tier2Threshold = 30,
                Tier2Bonus = 1500m,
                Tier3Threshold = 50,
                Tier3Bonus = 3000m,
                Currency = "EGP",
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
