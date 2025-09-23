using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Relationships;

namespace SynQcore.Infrastructure.Data.Configurations.Relationships;

public class ReportingRelationshipConfiguration : IEntityTypeConfiguration<ReportingRelationship>
{
    public void Configure(EntityTypeBuilder<ReportingRelationship> builder)
    {
        builder.ToTable("ReportingRelationships");
        
        // Chave primária
        builder.HasKey(r => r.Id);

        // Enum
        builder.Property(r => r.Type)
            .HasConversion<int>();

        // Relacionamento Manager -> DirectReports (1:N)
        builder.HasOne(r => r.Manager)
            .WithMany(e => e.DirectReports)
            .HasForeignKey(r => r.ManagerId)
            .OnDelete(DeleteBehavior.Restrict); // Evitar cascade para não dar problema

        // Relacionamento Subordinate -> ManagerRelationships (1:N)  
        builder.HasOne(r => r.Subordinate)
            .WithMany(e => e.ManagerRelationships)
            .HasForeignKey(r => r.SubordinateId)
            .OnDelete(DeleteBehavior.Restrict); // Evitar cascade

        // Relacionamento com Department - OPCIONAL
        builder.HasOne(r => r.Department)
            .WithMany()
            .HasForeignKey(r => r.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacionamento com Team - OPCIONAL
        builder.HasOne(r => r.Team)
            .WithMany()
            .HasForeignKey(r => r.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices para performance
        builder.HasIndex(r => r.ManagerId);
        builder.HasIndex(r => r.SubordinateId);
        builder.HasIndex(r => new { r.ManagerId, r.SubordinateId }).IsUnique();

        // Propriedades calculadas (não mapeadas)
        builder.Ignore(r => r.IsCurrentRelationship);
        builder.Ignore(r => r.RelationshipDuration);
    }
}