using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations.Communication;

/// <summary>
/// Configuração EF Core para sistema de endorsements corporativos
/// </summary>
public class EndorsementConfiguration : IEntityTypeConfiguration<Endorsement>
{
    public void Configure(EntityTypeBuilder<Endorsement> builder)
    {
        // Propriedades
        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.Note)
            .HasMaxLength(500);

        builder.Property(e => e.Context)
            .HasMaxLength(100);

        builder.Property(e => e.IsPublic)
            .HasDefaultValue(true);

        builder.Property(e => e.EndorsedAt)
            .IsRequired();

        // Índices para performance
        builder.HasIndex(e => e.PostId);
        builder.HasIndex(e => e.CommentId);
        builder.HasIndex(e => e.EndorserId);
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.EndorsedAt);
        builder.HasIndex(e => e.Context);

        // Índice composto para evitar duplicatas
        builder.HasIndex(e => new { e.PostId, e.EndorserId })
            .HasDatabaseName("IX_Endorsements_Post_Endorser_Unique")
            .IsUnique()
            .HasFilter("\"PostId\" IS NOT NULL");

        builder.HasIndex(e => new { e.CommentId, e.EndorserId })
            .HasDatabaseName("IX_Endorsements_Comment_Endorser_Unique")
            .IsUnique()
            .HasFilter("\"CommentId\" IS NOT NULL");

        // Relacionamentos
        builder.HasOne(e => e.Post)
            .WithMany() // Post terá navigation property para Endorsements
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Comment)
            .WithMany() // Comment terá navigation property para Endorsements
            .HasForeignKey(e => e.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Endorser)
            .WithMany() // Employee pode ter muitos endorsements dados
            .HasForeignKey(e => e.EndorserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Constraint: deve endossar OU post OU comment, nunca ambos
        builder.ToTable("Endorsements", "Communication", t =>
        {
            t.HasCheckConstraint(
                "CK_Endorsement_ContentType",
                "(\"PostId\" IS NOT NULL AND \"CommentId\" IS NULL) OR (\"PostId\" IS NULL AND \"CommentId\" IS NOT NULL)"
            );
        });

        // Query Filter para soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}