using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations;

public class CommentMentionConfiguration : IEntityTypeConfiguration<CommentMention>
{
    public void Configure(EntityTypeBuilder<CommentMention> builder)
    {
        builder.ToTable("CommentMentions", "Communication");

        // Propriedades básicas
        builder.Property(e => e.MentionText)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.StartPosition)
            .IsRequired();

        builder.Property(e => e.Length)
            .IsRequired();

        builder.Property(e => e.HasBeenNotified)
            .HasDefaultValue(false);

        builder.Property(e => e.IsRead)
            .HasDefaultValue(false);

        // Enum configurations com sentinel values
        builder.Property(e => e.Context)
            .HasConversion<int>()
            .HasDefaultValue(MentionContext.General);

        builder.Property(e => e.Urgency)
            .HasConversion<int>()
            .HasDefaultValue(MentionUrgency.Normal)
            .HasSentinel(MentionUrgency.Low); // Define Low como sentinel value

        // Configurar propriedades FK explicitamente
        builder.Property(e => e.CommentId)
            .IsRequired();
            
        builder.Property(e => e.MentionedEmployeeId)
            .IsRequired();
            
        builder.Property(e => e.MentionedById)
            .IsRequired();

        // Relacionamentos explícitos com ForeignKey names únicos
        builder.HasOne(e => e.Comment)
            .WithMany(c => c.Mentions) // Menções no comentário
            .HasForeignKey(e => e.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.MentionedEmployee)
            .WithMany() // Sem navigation collection para evitar ambiguidade
            .HasForeignKey(e => e.MentionedEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.MentionedBy)
            .WithMany() // Usar WithMany() vazio para evitar conflito com Employee.MentionsMade
            .HasForeignKey(e => e.MentionedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para performance
        builder.HasIndex(e => e.CommentId);
        builder.HasIndex(e => e.MentionedEmployeeId);
        builder.HasIndex(e => e.MentionedById);
        builder.HasIndex(e => new { e.MentionedEmployeeId, e.IsRead });
        builder.HasIndex(e => new { e.MentionedEmployeeId, e.HasBeenNotified });
        builder.HasIndex(e => e.CreatedAt);
    }
}