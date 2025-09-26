using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations.Notifications;

/// <summary>
/// Configuração EF Core para NotificationTemplate
/// </summary>
public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");

        builder.HasKey(x => x.Id);

        // Propriedades básicas
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(50);

        // Templates
        builder.Property(x => x.TitleTemplate)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ContentTemplate)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.EmailTemplate)
            .HasMaxLength(8000)
            .IsRequired(false);

        // Configurações padrão - usar valores inteiros para enums
        builder.Property(x => x.DefaultType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.DefaultPriority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.DefaultChannels)
            .HasConversion<int>()
            .IsRequired();

        // Flags padrão
        builder.Property(x => x.DefaultRequiresApproval)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DefaultRequiresAcknowledgment)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Metadados
        builder.Property(x => x.AvailablePlaceholders)
            .HasMaxLength(2000)
            .IsRequired(false);

        // Índices para performance e unicidade
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_NotificationTemplates_Code_Unique");

        builder.HasIndex(x => x.Category)
            .HasDatabaseName("IX_NotificationTemplates_Category");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_NotificationTemplates_IsActive");

        builder.HasIndex(x => x.DefaultType)
            .HasDatabaseName("IX_NotificationTemplates_DefaultType");

        // Índice composto para busca por categoria e status
        builder.HasIndex(x => new { x.Category, x.IsActive })
            .HasDatabaseName("IX_NotificationTemplates_Category_IsActive");
    }
}