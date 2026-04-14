using EduCore.Domain.Entities.TranslationModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.Configurations
{
    public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
    {
        public void Configure(EntityTypeBuilder<Translation> builder)
        {
            builder.ToTable("translations");
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => new { t.EntityType, t.EntityId, t.Field, t.Lang })
                   .IsUnique();
            builder.Property(t => t.EntityType).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Field).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Lang).IsRequired().HasMaxLength(10);
            builder.Property(t => t.Value).IsRequired().HasColumnType("NVARCHAR(MAX)");
        }
    }
}
