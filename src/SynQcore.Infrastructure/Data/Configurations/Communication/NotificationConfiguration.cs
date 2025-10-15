using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations.Communication;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        // Chave primária
        builder.HasKey(n => n.Id);

        // Propriedades obrigatórias
        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(500);

        // Enums
        builder.Property(n => n.Type)
            .HasConversion<int>();

        builder.Property(n => n.Priority)
            .HasConversion<int>();

        // Relacionamento com Employee (Recipient) - OBRIGATÓRIO
        builder.HasOne(n => n.Recipient)
            .WithMany(e => e.ReceivedNotifications)
            .HasForeignKey(n => n.RecipientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento com Employee (Sender) - OPCIONAL
        builder.HasOne(n => n.Sender)
            .WithMany(e => e.SentNotifications)
            .HasForeignKey(n => n.SenderId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacionamento com Post - OPCIONAL
        builder.HasOne(n => n.Post)
            .WithMany()
            .HasForeignKey(n => n.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento com Comment - OPCIONAL
        builder.HasOne(n => n.Comment)
            .WithMany()
            .HasForeignKey(n => n.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices para performance
        builder.HasIndex(n => n.RecipientId);
        builder.HasIndex(n => new { n.RecipientId, n.IsRead });
        builder.HasIndex(n => n.Type);
    }
}
