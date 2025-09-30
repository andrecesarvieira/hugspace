/*
 * SynQcore - Corporate Social Network
 *
 * Moderation Entity Configuration - Configuração EF Core para moderação
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para entidade de moderação
/// </summary>
public class ModerationEntityConfiguration : IEntityTypeConfiguration<ModerationEntity>
{
    public void Configure(EntityTypeBuilder<ModerationEntity> builder)
    {
        // Configuração da tabela
        builder.ToTable("Moderations");

        // Chave primária
        builder.HasKey(m => m.Id);

        // Propriedades obrigatórias
        builder.Property(m => m.ContentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.ContentId)
            .IsRequired();

        builder.Property(m => m.ContentAuthorId)
            .IsRequired();

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(ModerationStatus.Pending);

        builder.Property(m => m.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(ModerationSeverity.Low);

        builder.Property(m => m.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        // Propriedades opcionais
        builder.Property(m => m.ActionTaken)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.ModeratorNotes)
            .HasMaxLength(2000);

        builder.Property(m => m.AdditionalDetails)
            .HasColumnType("jsonb"); // PostgreSQL JSON

        builder.Property(m => m.IsAutomaticDetection)
            .HasDefaultValue(false);

        builder.Property(m => m.AutoDetectionScore)
            .HasColumnType("smallint");

        builder.Property(m => m.UserIpAddress)
            .HasMaxLength(45);

        builder.Property(m => m.UserAgent)
            .HasMaxLength(500);

        // Índices para performance
        builder.HasIndex(m => m.ContentId)
            .HasDatabaseName("IX_Moderations_ContentId");

        builder.HasIndex(m => m.ContentAuthorId)
            .HasDatabaseName("IX_Moderations_ContentAuthorId");

        builder.HasIndex(m => m.Status)
            .HasDatabaseName("IX_Moderations_Status");

        builder.HasIndex(m => m.Category)
            .HasDatabaseName("IX_Moderations_Category");

        builder.HasIndex(m => m.Severity)
            .HasDatabaseName("IX_Moderations_Severity");

        builder.HasIndex(m => m.ModeratorId)
            .HasDatabaseName("IX_Moderations_ModeratorId");

        builder.HasIndex(m => m.CreatedAt)
            .HasDatabaseName("IX_Moderations_CreatedAt");

        builder.HasIndex(m => new { m.Status, m.CreatedAt })
            .HasDatabaseName("IX_Moderations_Status_CreatedAt");

        builder.HasIndex(m => new { m.Category, m.Severity })
            .HasDatabaseName("IX_Moderations_Category_Severity");

        // Relacionamentos
        builder.HasOne(m => m.ContentAuthor)
            .WithMany()
            .HasForeignKey(m => m.ContentAuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.ReportedByEmployee)
            .WithMany()
            .HasForeignKey(m => m.ReportedByEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.Moderator)
            .WithMany()
            .HasForeignKey(m => m.ModeratorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(m => m.ModerationLogs)
            .WithOne(ml => ml.Moderation)
            .HasForeignKey(ml => ml.ModerationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Appeals)
            .WithOne(a => a.Moderation)
            .HasForeignKey(a => a.ModerationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurações de auditoria
        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(m => m.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
