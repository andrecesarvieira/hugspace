using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Configuração EF Core para a entidade CorporateDocument
/// </summary>
public class CorporateDocumentConfiguration : IEntityTypeConfiguration<CorporateDocument>
{
    public void Configure(EntityTypeBuilder<CorporateDocument> builder)
    {
        // Configuração da tabela
        builder.ToTable("CorporateDocuments");

        // Chave primária
        builder.HasKey(d => d.Id);

        // Propriedades obrigatórias
        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.OriginalFileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.StorageFileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Version)
            .IsRequired()
            .HasMaxLength(20);

        // Propriedades opcionais com tamanhos definidos
        builder.Property(d => d.Description)
            .HasMaxLength(2000);

        builder.Property(d => d.Tags)
            .HasMaxLength(1000);

        builder.Property(d => d.FileHash)
            .HasMaxLength(100);

        builder.Property(d => d.ExternalStorageUrl)
            .HasMaxLength(2000);

        builder.Property(d => d.Metadata)
            .HasColumnType("jsonb"); // PostgreSQL JSON binary

        // Enums com valores inteiros
        builder.Property(d => d.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(DocumentStatus.Draft)
            .HasSentinel(DocumentStatus.Draft);

        builder.Property(d => d.AccessLevel)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(DocumentAccessLevel.Internal)
            .HasSentinel(DocumentAccessLevel.Internal);

        builder.Property(d => d.Category)
            .HasConversion<int>()
            .IsRequired();

        // Valores padrão
        builder.Property(d => d.IsCurrentVersion)
            .HasDefaultValue(true);

        builder.Property(d => d.DownloadCount)
            .HasDefaultValue(0);

        // Relacionamentos
        builder.HasOne(d => d.UploadedByEmployee)
            .WithMany()
            .HasForeignKey(d => d.UploadedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.OwnerDepartment)
            .WithMany()
            .HasForeignKey(d => d.OwnerDepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.ApprovedByEmployee)
            .WithMany()
            .HasForeignKey(d => d.ApprovedByEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Auto-relacionamento para versionamento
        builder.HasOne(d => d.ParentDocument)
            .WithMany(d => d.ChildVersions)
            .HasForeignKey(d => d.ParentDocumentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacionamentos com coleções
        builder.HasMany(d => d.DocumentAccesses)
            .WithOne(da => da.Document)
            .HasForeignKey(da => da.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.AccessLogs)
            .WithOne(al => al.Document)
            .HasForeignKey(al => al.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices para performance
        builder.HasIndex(d => d.UploadedByEmployeeId)
            .HasDatabaseName("IX_CorporateDocuments_UploadedByEmployeeId");

        builder.HasIndex(d => d.OwnerDepartmentId)
            .HasDatabaseName("IX_CorporateDocuments_OwnerDepartmentId");

        builder.HasIndex(d => d.Type)
            .HasDatabaseName("IX_CorporateDocuments_Type");

        builder.HasIndex(d => d.Status)
            .HasDatabaseName("IX_CorporateDocuments_Status");

        builder.HasIndex(d => d.AccessLevel)
            .HasDatabaseName("IX_CorporateDocuments_AccessLevel");

        builder.HasIndex(d => d.Category)
            .HasDatabaseName("IX_CorporateDocuments_Category");

        builder.HasIndex(d => d.IsCurrentVersion)
            .HasDatabaseName("IX_CorporateDocuments_IsCurrentVersion");

        builder.HasIndex(d => d.ParentDocumentId)
            .HasDatabaseName("IX_CorporateDocuments_ParentDocumentId");

        builder.HasIndex(d => new { d.Title, d.Type })
            .HasDatabaseName("IX_CorporateDocuments_Title_Type");

        builder.HasIndex(d => new { d.Status, d.AccessLevel })
            .HasDatabaseName("IX_CorporateDocuments_Status_AccessLevel");

        // Índice de busca de título (B-tree para PostgreSQL)
        builder.HasIndex(d => d.Title)
            .HasDatabaseName("IX_CorporateDocuments_Title_Search");

        // Soft delete
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
