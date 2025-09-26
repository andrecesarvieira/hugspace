using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations.Notifications;

/// <summary>
/// Configuração EF Core para NotificationDelivery
/// </summary>
public class NotificationDeliveryConfiguration : IEntityTypeConfiguration<NotificationDelivery>
{
    public void Configure(EntityTypeBuilder<NotificationDelivery> builder)
    {
        builder.ToTable("NotificationDeliveries");

        builder.HasKey(x => x.Id);

        // Enums - usar valores inteiros
        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Channel)
            .HasConversion<int>()
            .IsRequired();

        // Timestamps
        builder.Property(x => x.DeliveredAt)
            .IsRequired(false);

        builder.Property(x => x.ReadAt)
            .IsRequired(false);

        builder.Property(x => x.AcknowledgedAt)
            .IsRequired(false);

        builder.Property(x => x.NextAttemptAt)
            .IsRequired(false);

        // Contadores e detalhes
        builder.Property(x => x.DeliveryAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.ErrorDetails)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.ChannelData)
            .HasMaxLength(500)
            .IsRequired(false);

        // Relacionamentos
        builder.HasOne(x => x.Notification)
            .WithMany(x => x.Deliveries)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices para performance
        builder.HasIndex(x => x.NotificationId)
            .HasDatabaseName("IX_NotificationDeliveries_NotificationId");

        builder.HasIndex(x => x.EmployeeId)
            .HasDatabaseName("IX_NotificationDeliveries_EmployeeId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_NotificationDeliveries_Status");

        builder.HasIndex(x => x.Channel)
            .HasDatabaseName("IX_NotificationDeliveries_Channel");

        builder.HasIndex(x => x.NextAttemptAt)
            .HasDatabaseName("IX_NotificationDeliveries_NextAttemptAt");

        // Índices compostos para consultas complexas
        builder.HasIndex(x => new { x.EmployeeId, x.Status })
            .HasDatabaseName("IX_NotificationDeliveries_EmployeeId_Status");

        builder.HasIndex(x => new { x.NotificationId, x.EmployeeId })
            .IsUnique()
            .HasDatabaseName("IX_NotificationDeliveries_NotificationId_EmployeeId_Unique");

        builder.HasIndex(x => new { x.Status, x.NextAttemptAt })
            .HasDatabaseName("IX_NotificationDeliveries_Status_NextAttemptAt");
    }
}