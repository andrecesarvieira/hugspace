/*
 * SynQcore - Corporate Social Network
 *
 * Moderation Log Entity Configuration - Configuração EF Core para logs de moderação
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para logs de moderação
/// </summary>
public class ModerationLogEntityConfiguration : IEntityTypeConfiguration<ModerationLogEntity>
{
    public void Configure(EntityTypeBuilder<ModerationLogEntity> builder)
    {
        // Configuração da tabela
        builder.ToTable("ModerationLogs");

        // Chave primária
        builder.HasKey(ml => ml.Id);

        // Propriedades obrigatórias
        builder.Property(ml => ml.ModerationId)
            .IsRequired();

        builder.Property(ml => ml.Action)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Propriedades opcionais
        builder.Property(ml => ml.PreviousStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ml => ml.NewStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ml => ml.Notes)
            .HasMaxLength(1000);

        builder.Property(ml => ml.AdditionalData)
            .HasColumnType("jsonb"); // PostgreSQL JSON

        builder.Property(ml => ml.UserIpAddress)
            .HasMaxLength(45);

        builder.Property(ml => ml.UserAgent)
            .HasMaxLength(500);

        builder.Property(ml => ml.IsAutomaticAction)
            .HasDefaultValue(false);

        // Índices para performance
        builder.HasIndex(ml => ml.ModerationId)
            .HasDatabaseName("IX_ModerationLogs_ModerationId");

        builder.HasIndex(ml => ml.ActionByEmployeeId)
            .HasDatabaseName("IX_ModerationLogs_ActionByEmployeeId");

        builder.HasIndex(ml => ml.Action)
            .HasDatabaseName("IX_ModerationLogs_Action");

        builder.HasIndex(ml => ml.CreatedAt)
            .HasDatabaseName("IX_ModerationLogs_CreatedAt");

        builder.HasIndex(ml => new { ml.ModerationId, ml.CreatedAt })
            .HasDatabaseName("IX_ModerationLogs_ModerationId_CreatedAt");

        builder.HasIndex(ml => new { ml.Action, ml.CreatedAt })
            .HasDatabaseName("IX_ModerationLogs_Action_CreatedAt");

        // Relacionamentos
        builder.HasOne(ml => ml.Moderation)
            .WithMany(m => m.ModerationLogs)
            .HasForeignKey(ml => ml.ModerationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ml => ml.ActionByEmployee)
            .WithMany()
            .HasForeignKey(ml => ml.ActionByEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configurações de auditoria
        builder.Property(ml => ml.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(ml => ml.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
