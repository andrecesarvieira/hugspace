using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations.Communication;

public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        builder.ToTable("PostTags", "Communication");

        // Chave composta
        builder.HasKey(pt => new { pt.PostId, pt.TagId });

        // Propriedades
        builder.Property(e => e.AddedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relacionamentos
        builder.HasOne(pt => pt.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(pt => pt.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.AddedBy)
            .WithMany()
            .HasForeignKey(pt => pt.AddedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(e => e.AddedAt);
        builder.HasIndex(e => e.AddedById);

        // Query Filter para soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
