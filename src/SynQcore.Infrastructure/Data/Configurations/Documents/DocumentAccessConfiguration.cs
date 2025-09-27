using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Configuração EF Core para a entidade DocumentAccess
/// </summary>
public class DocumentAccessConfiguration : IEntityTypeConfiguration<DocumentAccess>
{
    public void Configure(EntityTypeBuilder<DocumentAccess> builder)
    {
        // Chave primária
        builder.HasKey(da => da.Id);

        // Propriedades
        builder.Property(da => da.Role)
            .HasMaxLength(100);

        builder.Property(da => da.Reason)
            .HasMaxLength(1000);

        // Enum com valor inteiro
        builder.Property(da => da.AccessType)
            .HasConversion<int>()
            .IsRequired();

        // Valores padrão
        builder.Property(da => da.IsActive)
            .HasDefaultValue(true);

        builder.Property(da => da.GrantedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relacionamentos
        builder.HasOne(da => da.Document)
            .WithMany(d => d.DocumentAccesses)
            .HasForeignKey(da => da.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(da => da.Employee)
            .WithMany()
            .HasForeignKey(da => da.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(da => da.Department)
            .WithMany()
            .HasForeignKey(da => da.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(da => da.GrantedByEmployee)
            .WithMany()
            .HasForeignKey(da => da.GrantedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para performance
        builder.HasIndex(da => da.DocumentId)
            .HasDatabaseName("IX_DocumentAccesses_DocumentId");

        builder.HasIndex(da => da.EmployeeId)
            .HasDatabaseName("IX_DocumentAccesses_EmployeeId");

        builder.HasIndex(da => da.DepartmentId)
            .HasDatabaseName("IX_DocumentAccesses_DepartmentId");

        builder.HasIndex(da => da.Role)
            .HasDatabaseName("IX_DocumentAccesses_Role");

        builder.HasIndex(da => da.AccessType)
            .HasDatabaseName("IX_DocumentAccesses_AccessType");

        builder.HasIndex(da => da.IsActive)
            .HasDatabaseName("IX_DocumentAccesses_IsActive");

        builder.HasIndex(da => new { da.DocumentId, da.EmployeeId })
            .HasDatabaseName("IX_DocumentAccesses_Document_Employee");

        builder.HasIndex(da => new { da.DocumentId, da.DepartmentId })
            .HasDatabaseName("IX_DocumentAccesses_Document_Department");

        builder.HasIndex(da => new { da.DocumentId, da.Role })
            .HasDatabaseName("IX_DocumentAccesses_Document_Role");

        // Soft delete
        builder.HasQueryFilter(da => !da.IsDeleted);

        // Configuração da tabela com constraint
        builder.ToTable("DocumentAccesses", t => 
            t.HasCheckConstraint("CK_DocumentAccesses_OneAccessType",
                "((\"EmployeeId\" IS NOT NULL) AND (\"DepartmentId\" IS NULL) AND (\"Role\" IS NULL)) OR " +
                "((\"EmployeeId\" IS NULL) AND (\"DepartmentId\" IS NOT NULL) AND (\"Role\" IS NULL)) OR " +
                "((\"EmployeeId\" IS NULL) AND (\"DepartmentId\" IS NULL) AND (\"Role\" IS NOT NULL))"));
    }
}