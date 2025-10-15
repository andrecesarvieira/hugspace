using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations.Communication;

// Configuração EF Core para entidade Comment - sistema de comentários corporativos
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments", "Communication");

        // Configuração da chave primária
        builder.HasKey(c => c.Id);

        // Propriedades obrigatórias
        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.Visibility)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.Priority)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.ModerationStatus)
            .IsRequired()
            .HasConversion<string>();

        // Propriedades opcionais
        builder.Property(c => c.ModerationReason)
            .HasMaxLength(1000);

        builder.Property(c => c.ResolutionNote)
            .HasMaxLength(500);

        // Relacionamento com Employee (Author) - OBRIGATÓRIO
        builder.HasOne(c => c.Author)
            .WithMany(e => e.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com Post - OBRIGATÓRIO  
        builder.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento com Employee (ModeratedBy) - OPCIONAL
        builder.HasOne(c => c.ModeratedBy)
            .WithMany()
            .HasForeignKey(c => c.ModeratedById)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacionamento com Employee (ResolvedBy) - OPCIONAL  
        builder.HasOne(c => c.ResolvedBy)
            .WithMany()
            .HasForeignKey(c => c.ResolvedById)
            .OnDelete(DeleteBehavior.SetNull);

        // Self-referencing relationship para replies (comentário pai)
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para performance
        builder.HasIndex(c => c.PostId);
        builder.HasIndex(c => c.AuthorId);
        builder.HasIndex(c => c.ParentCommentId);
        builder.HasIndex(c => c.ModerationStatus);
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.CreatedAt);

        // Índice composto para queries de threads
        builder.HasIndex(c => new { c.PostId, c.ParentCommentId, c.CreatedAt });

        // Soft delete
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
