using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations.Communication;

public class KnowledgeCategoryConfiguration : IEntityTypeConfiguration<KnowledgeCategory>
{
    public void Configure(EntityTypeBuilder<KnowledgeCategory> builder)
    {
        builder.ToTable("KnowledgeCategories", "Communication");

        // Propriedades
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.Color)
            .IsRequired()
            .HasMaxLength(7)
            .HasDefaultValue("#007ACC");

        builder.Property(e => e.Icon)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("📄");

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        // Índices
        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.HasIndex(e => e.ParentCategoryId);

        // Relacionamentos
        builder.HasOne(e => e.ParentCategory)
            .WithMany(e => e.SubCategories)
            .HasForeignKey(e => e.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Posts)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Query Filter para soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
