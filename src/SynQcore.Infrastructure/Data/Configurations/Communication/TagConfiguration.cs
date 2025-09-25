using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations.Communication;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags", "Communication");

        // Propriedades
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.Color)
            .IsRequired()
            .HasMaxLength(7)
            .HasDefaultValue("#6B7280");

        builder.Property(e => e.UsageCount)
            .HasDefaultValue(0);

        // Índices
        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.HasIndex(e => e.Type);

        builder.HasIndex(e => e.UsageCount);

        // Query Filter para soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}