using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations.Communication;

public class FeedEntryConfiguration : IEntityTypeConfiguration<FeedEntry>
{
    public void Configure(EntityTypeBuilder<FeedEntry> builder)
    {
        // Tabela
        builder.ToTable("FeedEntries");

        // Chave primária
        builder.HasKey(f => f.Id);

        // Propriedades obrigatórias
        builder.Property(f => f.UserId)
            .IsRequired();

        builder.Property(f => f.PostId)
            .IsRequired();

        builder.Property(f => f.AuthorId)
            .IsRequired();

        builder.Property(f => f.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(f => f.Reason)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(f => f.RelevanceScore)
            .IsRequired()
            .HasPrecision(5, 4) // 0.0000 to 1.0000
            .HasDefaultValue(0.5);

        // Propriedades opcionais
        builder.Property(f => f.ViewedAt)
            .IsRequired(false);

        // Relacionamentos
        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Post)
            .WithMany()
            .HasForeignKey(f => f.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Author)
            .WithMany()
            .HasForeignKey(f => f.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Department)
            .WithMany()
            .HasForeignKey(f => f.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(f => f.Team)
            .WithMany()
            .HasForeignKey(f => f.TeamId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Índices para performance
        // Consulta principal: feed por usuário ordenado por relevância/data
        builder.HasIndex(f => new { f.UserId, f.CreatedAt })
            .HasDatabaseName("IX_FeedEntries_UserId_CreatedAt")
            .IsDescending(false, true);

        // Consulta por usuário e prioridade
        builder.HasIndex(f => new { f.UserId, f.Priority, f.RelevanceScore })
            .HasDatabaseName("IX_FeedEntries_UserId_Priority_Relevance")
            .IsDescending(false, true, true);

        // Limpeza de feeds antigos
        builder.HasIndex(f => new { f.CreatedAt, f.IsRead })
            .HasDatabaseName("IX_FeedEntries_CreatedAt_IsRead");

        // Consulta por post (para análise)
        builder.HasIndex(f => f.PostId)
            .HasDatabaseName("IX_FeedEntries_PostId");

        // Consulta por autor (para análise de reach)
        builder.HasIndex(f => f.AuthorId)
            .HasDatabaseName("IX_FeedEntries_AuthorId");
    }
}
