/*
 * SynQcore - Corporate Social Network API
 *
 * Audit Log Entity Configuration for Entity Framework
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade AuditLogEntity para Entity Framework
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogEntity>
{
    public void Configure(EntityTypeBuilder<AuditLogEntity> builder)
    {
        builder.ToTable("AuditLogs");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(x => x.UserId)
            .HasMaxLength(450); // Same as ASP.NET Identity

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.UserRole)
            .HasMaxLength(100);

        builder.Property(x => x.ActionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.ResourceType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ResourceId)
            .HasMaxLength(450);

        builder.Property(x => x.Details)
            .HasColumnType("text"); // JSON storage

        builder.Property(x => x.Success)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(x => x.ClientIpAddress)
            .HasMaxLength(45); // IPv6 support

        builder.Property(x => x.UserAgent)
            .HasMaxLength(1000);

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(100);

        builder.Property(x => x.RequestPath)
            .HasMaxLength(500);

        builder.Property(x => x.HttpMethod)
            .HasMaxLength(10);

        builder.Property(x => x.DurationMs);

        builder.Property(x => x.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.Tags)
            .HasMaxLength(500);

        builder.Property(x => x.RetentionDate);

        builder.Property(x => x.RequiresAttention)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.ReviewedAt);

        builder.Property(x => x.ReviewedBy)
            .HasMaxLength(450);

        builder.Property(x => x.ReviewNotes)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId");

        builder.HasIndex(x => x.ActionType)
            .HasDatabaseName("IX_AuditLogs_ActionType");

        builder.HasIndex(x => x.ResourceType)
            .HasDatabaseName("IX_AuditLogs_ResourceType");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_AuditLogs_CreatedAt");

        builder.HasIndex(x => x.Severity)
            .HasDatabaseName("IX_AuditLogs_Severity");

        builder.HasIndex(x => x.Category)
            .HasDatabaseName("IX_AuditLogs_Category");

        builder.HasIndex(x => x.ClientIpAddress)
            .HasDatabaseName("IX_AuditLogs_ClientIpAddress");

        builder.HasIndex(x => x.RequiresAttention)
            .HasDatabaseName("IX_AuditLogs_RequiresAttention");

        builder.HasIndex(x => x.RetentionDate)
            .HasDatabaseName("IX_AuditLogs_RetentionDate");

        // Composite indexes for common queries
        builder.HasIndex(x => new { x.UserId, x.CreatedAt })
            .HasDatabaseName("IX_AuditLogs_UserId_CreatedAt");

        builder.HasIndex(x => new { x.ActionType, x.CreatedAt })
            .HasDatabaseName("IX_AuditLogs_ActionType_CreatedAt");

        builder.HasIndex(x => new { x.Severity, x.RequiresAttention })
            .HasDatabaseName("IX_AuditLogs_Severity_RequiresAttention");
    }
}
