/*
 * SynQcore - Corporate Social Network
 *
 * User Punishment Entity Configuration - Configuração EF Core para punições de usuários
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para punições de usuários
/// </summary>
public class UserPunishmentEntityConfiguration : IEntityTypeConfiguration<UserPunishmentEntity>
{
    public void Configure(EntityTypeBuilder<UserPunishmentEntity> builder)
    {
        // Configuração da tabela
        builder.ToTable("UserPunishments");

        // Chave primária
        builder.HasKey(up => up.Id);

        // Propriedades obrigatórias
        builder.Property(up => up.EmployeeId)
            .IsRequired();

        builder.Property(up => up.AppliedByEmployeeId)
            .IsRequired();

        builder.Property(up => up.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(up => up.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(PunishmentStatus.Active);

        builder.Property(up => up.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(up => up.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(up => up.StartDate)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Propriedades opcionais
        builder.Property(up => up.Details)
            .HasMaxLength(2000);

        builder.Property(up => up.Restrictions)
            .HasColumnType("jsonb"); // PostgreSQL JSON

        builder.Property(up => up.IsPermanent)
            .HasDefaultValue(false);

        builder.Property(up => up.PreviousWarnings)
            .HasDefaultValue(0);

        builder.Property(up => up.IsRecurrence)
            .HasDefaultValue(false);

        builder.Property(up => up.InfractionPoints)
            .HasDefaultValue(0);

        builder.Property(up => up.NotificationMethod)
            .HasMaxLength(50);

        builder.Property(up => up.ModeratorIpAddress)
            .HasMaxLength(45);

        builder.Property(up => up.AdditionalData)
            .HasColumnType("jsonb"); // PostgreSQL JSON

        // Índices para performance
        builder.HasIndex(up => up.EmployeeId)
            .HasDatabaseName("IX_UserPunishments_EmployeeId");

        builder.HasIndex(up => up.Type)
            .HasDatabaseName("IX_UserPunishments_Type");

        builder.Property(up => up.Status)
            .HasDefaultValue(PunishmentStatus.Active);

        builder.HasIndex(up => up.Severity)
            .HasDatabaseName("IX_UserPunishments_Severity");

        builder.HasIndex(up => up.StartDate)
            .HasDatabaseName("IX_UserPunishments_StartDate");

        builder.HasIndex(up => up.EndDate)
            .HasDatabaseName("IX_UserPunishments_EndDate");

        builder.HasIndex(up => up.AppliedByEmployeeId)
            .HasDatabaseName("IX_UserPunishments_AppliedByEmployeeId");

        builder.HasIndex(up => up.ModerationId)
            .HasDatabaseName("IX_UserPunishments_ModerationId");

        builder.HasIndex(up => new { up.EmployeeId, up.Status })
            .HasDatabaseName("IX_UserPunishments_EmployeeId_Status");

        builder.HasIndex(up => new { up.Type, up.Status })
            .HasDatabaseName("IX_UserPunishments_Type_Status");

        builder.HasIndex(up => new { up.StartDate, up.EndDate })
            .HasDatabaseName("IX_UserPunishments_StartDate_EndDate");

        // Relacionamentos
        builder.HasOne(up => up.Employee)
            .WithMany()
            .HasForeignKey(up => up.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(up => up.Moderation)
            .WithMany()
            .HasForeignKey(up => up.ModerationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(up => up.AppliedByEmployee)
            .WithMany()
            .HasForeignKey(up => up.AppliedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurações de auditoria
        builder.Property(up => up.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(up => up.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
