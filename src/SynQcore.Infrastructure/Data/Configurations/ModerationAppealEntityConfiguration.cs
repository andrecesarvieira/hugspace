/*
 * SynQcore - Corporate Social Network
 *
 * Moderation Appeal Entity Configuration - Configuração EF Core para appeals de moderação
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para appeals de moderação
/// </summary>
public class ModerationAppealEntityConfiguration : IEntityTypeConfiguration<ModerationAppealEntity>
{
    public void Configure(EntityTypeBuilder<ModerationAppealEntity> builder)
    {
        // Configuração da tabela
        builder.ToTable("ModerationAppeals");

        // Chave primária
        builder.HasKey(ma => ma.Id);

        // Propriedades obrigatórias
        builder.Property(ma => ma.ModerationId)
            .IsRequired();

        builder.Property(ma => ma.AppealByEmployeeId)
            .IsRequired();

        builder.Property(ma => ma.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(AppealStatus.Pending);

        builder.Property(ma => ma.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ma => ma.Reason)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(ma => ma.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(AppealPriority.Normal)
            .HasSentinel(AppealPriority.None); // Usar None como valor sentinela

        // Propriedades opcionais
        builder.Property(ma => ma.Evidence)
            .HasMaxLength(5000);

        builder.Property(ma => ma.ReviewerResponse)
            .HasMaxLength(2000);

        builder.Property(ma => ma.Decision)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ma => ma.ResultAction)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ma => ma.AdditionalData)
            .HasColumnType("jsonb"); // PostgreSQL JSON

        builder.Property(ma => ma.UserIpAddress)
            .HasMaxLength(45);

        builder.Property(ma => ma.UserAgent)
            .HasMaxLength(500);

        builder.Property(ma => ma.IsAnonymous)
            .HasDefaultValue(false);

        // Índices para performance
        builder.HasIndex(ma => ma.ModerationId)
            .HasDatabaseName("IX_ModerationAppeals_ModerationId");

        builder.HasIndex(ma => ma.AppealByEmployeeId)
            .HasDatabaseName("IX_ModerationAppeals_AppealByEmployeeId");

        builder.HasIndex(ma => ma.Status)
            .HasDatabaseName("IX_ModerationAppeals_Status");

        builder.HasIndex(ma => ma.Type)
            .HasDatabaseName("IX_ModerationAppeals_Type");

        builder.HasIndex(ma => ma.Priority)
            .HasDatabaseName("IX_ModerationAppeals_Priority");

        builder.HasIndex(ma => ma.ReviewedByEmployeeId)
            .HasDatabaseName("IX_ModerationAppeals_ReviewedByEmployeeId");

        builder.HasIndex(ma => ma.CreatedAt)
            .HasDatabaseName("IX_ModerationAppeals_CreatedAt");

        builder.HasIndex(ma => new { ma.Status, ma.Priority })
            .HasDatabaseName("IX_ModerationAppeals_Status_Priority");

        builder.HasIndex(ma => new { ma.Status, ma.CreatedAt })
            .HasDatabaseName("IX_ModerationAppeals_Status_CreatedAt");

        // Relacionamentos
        builder.HasOne(ma => ma.Moderation)
            .WithMany(m => m.Appeals)
            .HasForeignKey(ma => ma.ModerationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ma => ma.AppealByEmployee)
            .WithMany()
            .HasForeignKey(ma => ma.AppealByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ma => ma.ReviewedByEmployee)
            .WithMany()
            .HasForeignKey(ma => ma.ReviewedByEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configurações de auditoria
        builder.Property(ma => ma.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(ma => ma.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
