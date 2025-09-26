using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations.Notifications;

/// <summary>
/// Configuração EF Core para CorporateNotification
/// </summary>
public class CorporateNotificationConfiguration : IEntityTypeConfiguration<CorporateNotification>
{
    public void Configure(EntityTypeBuilder<CorporateNotification> builder)
    {
        builder.ToTable("CorporateNotifications");

        builder.HasKey(x => x.Id);

        // Propriedades básicas
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(4000);

        // Enums - usar valores inteiros para evitar warnings
        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.EnabledChannels)
            .HasConversion<int>()
            .IsRequired();

        // Timestamps
        builder.Property(x => x.ExpiresAt)
            .IsRequired(false);

        builder.Property(x => x.ScheduledFor)
            .IsRequired(false);

        builder.Property(x => x.ApprovedAt)
            .IsRequired(false);

        // Flags
        builder.Property(x => x.RequiresApproval)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.RequiresAcknowledgment)
            .IsRequired()
            .HasDefaultValue(false);

        // Metadados JSON
        builder.Property(x => x.Metadata)
            .HasMaxLength(2000)
            .IsRequired(false);

        // Relacionamentos
        builder.HasOne(x => x.CreatedByEmployee)
            .WithMany()
            .HasForeignKey(x => x.CreatedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TargetDepartment)
            .WithMany()
            .HasForeignKey(x => x.TargetDepartmentId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(x => x.ApprovedByEmployee)
            .WithMany()
            .HasForeignKey(x => x.ApprovedByEmployeeId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Navegação para entregas
        builder.HasMany(x => x.Deliveries)
            .WithOne(x => x.Notification)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices para performance
        builder.HasIndex(x => x.Type)
            .HasDatabaseName("IX_CorporateNotifications_Type");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_CorporateNotifications_Status");

        builder.HasIndex(x => x.Priority)
            .HasDatabaseName("IX_CorporateNotifications_Priority");

        builder.HasIndex(x => x.CreatedByEmployeeId)
            .HasDatabaseName("IX_CorporateNotifications_CreatedByEmployeeId");

        builder.HasIndex(x => x.TargetDepartmentId)
            .HasDatabaseName("IX_CorporateNotifications_TargetDepartmentId");

        builder.HasIndex(x => x.ScheduledFor)
            .HasDatabaseName("IX_CorporateNotifications_ScheduledFor");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_CorporateNotifications_ExpiresAt");

        // Índice composto para busca eficiente
        builder.HasIndex(x => new { x.Status, x.ScheduledFor })
            .HasDatabaseName("IX_CorporateNotifications_Status_ScheduledFor");
    }
}