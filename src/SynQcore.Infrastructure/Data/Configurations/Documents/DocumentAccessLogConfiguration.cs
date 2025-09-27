using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities;

namespace SynQcore.Infrastructure.Data.Configurations.Documents;

/// <summary>
/// Configuração EF Core para a entidade DocumentAccessLog
/// </summary>
public class DocumentAccessLogConfiguration : IEntityTypeConfiguration<DocumentAccessLog>
{
    public void Configure(EntityTypeBuilder<DocumentAccessLog> builder)
    {
        // Configuração da tabela
        builder.ToTable("DocumentAccessLogs");

        // Chave primária
        builder.HasKey(dal => dal.Id);

        // Propriedades
        builder.Property(dal => dal.IpAddress)
            .HasMaxLength(50);

        builder.Property(dal => dal.UserAgent)
            .HasMaxLength(500);

        builder.Property(dal => dal.Details)
            .HasMaxLength(2000);

        // Enum com valor inteiro
        builder.Property(dal => dal.Action)
            .HasConversion<int>()
            .IsRequired();

        // Valor padrão para data de acesso
        builder.Property(dal => dal.AccessedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relacionamentos
        builder.HasOne(dal => dal.Document)
            .WithMany(d => d.AccessLogs)
            .HasForeignKey(dal => dal.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dal => dal.Employee)
            .WithMany()
            .HasForeignKey(dal => dal.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices para performance e auditoria
        builder.HasIndex(dal => dal.DocumentId)
            .HasDatabaseName("IX_DocumentAccessLogs_DocumentId");

        builder.HasIndex(dal => dal.EmployeeId)
            .HasDatabaseName("IX_DocumentAccessLogs_EmployeeId");

        builder.HasIndex(dal => dal.Action)
            .HasDatabaseName("IX_DocumentAccessLogs_Action");

        builder.HasIndex(dal => dal.AccessedAt)
            .HasDatabaseName("IX_DocumentAccessLogs_AccessedAt");

        builder.HasIndex(dal => new { dal.DocumentId, dal.AccessedAt })
            .HasDatabaseName("IX_DocumentAccessLogs_Document_AccessedAt");

        builder.HasIndex(dal => new { dal.EmployeeId, dal.AccessedAt })
            .HasDatabaseName("IX_DocumentAccessLogs_Employee_AccessedAt");

        builder.HasIndex(dal => new { dal.Action, dal.AccessedAt })
            .HasDatabaseName("IX_DocumentAccessLogs_Action_AccessedAt");

        // Índice composto para relatórios de auditoria
        builder.HasIndex(dal => new { dal.DocumentId, dal.EmployeeId, dal.Action, dal.AccessedAt })
            .HasDatabaseName("IX_DocumentAccessLogs_Audit");

        // Soft delete (opcional para logs - normalmente logs são mantidos)
        builder.HasQueryFilter(dal => !dal.IsDeleted);
    }
}

/// <summary>
/// Configuração EF Core para a entidade DocumentTemplate
/// </summary>
public class DocumentTemplateConfiguration : IEntityTypeConfiguration<DocumentTemplate>
{
    public void Configure(EntityTypeBuilder<DocumentTemplate> builder)
    {
        // Configuração da tabela
        builder.ToTable("DocumentTemplates");

        // Chave primária
        builder.HasKey(dt => dt.Id);

        // Propriedades obrigatórias
        builder.Property(dt => dt.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(dt => dt.TemplateFileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(dt => dt.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(dt => dt.Version)
            .IsRequired()
            .HasMaxLength(20);

        // Propriedades opcionais
        builder.Property(dt => dt.Description)
            .HasMaxLength(2000);

        builder.Property(dt => dt.StorageUrl)
            .HasMaxLength(2000);

        builder.Property(dt => dt.Placeholders)
            .HasColumnType("jsonb"); // PostgreSQL JSON binary

        builder.Property(dt => dt.UsageInstructions)
            .HasMaxLength(5000);

        builder.Property(dt => dt.Tags)
            .HasMaxLength(1000);

        // Enums com valores inteiros
        builder.Property(dt => dt.DocumentType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(dt => dt.DefaultCategory)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(dt => dt.DefaultAccessLevel)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(DocumentAccessLevel.Internal);

        // Valores padrão
        builder.Property(dt => dt.IsActive)
            .HasDefaultValue(true);

        builder.Property(dt => dt.IsDefault)
            .HasDefaultValue(false);

        builder.Property(dt => dt.UsageCount)
            .HasDefaultValue(0);

        // Relacionamentos
        builder.HasOne(dt => dt.OwnerDepartment)
            .WithMany()
            .HasForeignKey(dt => dt.OwnerDepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(dt => dt.CreatedByEmployee)
            .WithMany()
            .HasForeignKey(dt => dt.CreatedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para performance
        builder.HasIndex(dt => dt.Name)
            .HasDatabaseName("IX_DocumentTemplates_Name");

        builder.HasIndex(dt => dt.DocumentType)
            .HasDatabaseName("IX_DocumentTemplates_DocumentType");

        builder.HasIndex(dt => dt.DefaultCategory)
            .HasDatabaseName("IX_DocumentTemplates_DefaultCategory");

        builder.HasIndex(dt => dt.IsActive)
            .HasDatabaseName("IX_DocumentTemplates_IsActive");

        builder.HasIndex(dt => dt.IsDefault)
            .HasDatabaseName("IX_DocumentTemplates_IsDefault");

        builder.HasIndex(dt => dt.OwnerDepartmentId)
            .HasDatabaseName("IX_DocumentTemplates_OwnerDepartmentId");

        builder.HasIndex(dt => new { dt.DocumentType, dt.IsActive })
            .HasDatabaseName("IX_DocumentTemplates_DocumentType_IsActive");

        builder.HasIndex(dt => new { dt.IsDefault, dt.DocumentType })
            .HasDatabaseName("IX_DocumentTemplates_IsDefault_DocumentType");

        // Soft delete
        builder.HasQueryFilter(dt => !dt.IsDeleted);

        // Constraint para garantir apenas um template padrão por tipo
        builder.HasIndex(dt => new { dt.DocumentType, dt.IsDefault })
            .HasDatabaseName("UX_DocumentTemplates_DefaultPerType")
            .IsUnique()
            .HasFilter("\"IsDefault\" = true AND \"IsDeleted\" = false");
    }
}

/// <summary>
/// Configuração EF Core para a entidade MediaAsset
/// </summary>
public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        // Configuração da tabela
        builder.ToTable("MediaAssets");

        // Chave primária
        builder.HasKey(ma => ma.Id);

        // Propriedades obrigatórias
        builder.Property(ma => ma.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ma => ma.OriginalFileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ma => ma.StorageFileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ma => ma.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        // Propriedades opcionais
        builder.Property(ma => ma.Description)
            .HasMaxLength(2000);

        builder.Property(ma => ma.StorageUrl)
            .HasMaxLength(2000);

        builder.Property(ma => ma.ThumbnailUrl)
            .HasMaxLength(2000);

        builder.Property(ma => ma.Tags)
            .HasMaxLength(1000);

        builder.Property(ma => ma.Metadata)
            .HasColumnType("jsonb"); // PostgreSQL JSON binary

        // Enums com valores inteiros
        builder.Property(ma => ma.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ma => ma.Category)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ma => ma.AccessLevel)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(DocumentAccessLevel.Internal);

        // Valores padrão
        builder.Property(ma => ma.IsApproved)
            .HasDefaultValue(false);

        builder.Property(ma => ma.DownloadCount)
            .HasDefaultValue(0);

        // Relacionamentos
        builder.HasOne(ma => ma.UploadedByEmployee)
            .WithMany()
            .HasForeignKey(ma => ma.UploadedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ma => ma.ApprovedByEmployee)
            .WithMany()
            .HasForeignKey(ma => ma.ApprovedByEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices para performance
        builder.HasIndex(ma => ma.Name)
            .HasDatabaseName("IX_MediaAssets_Name");

        builder.HasIndex(ma => ma.Type)
            .HasDatabaseName("IX_MediaAssets_Type");

        builder.HasIndex(ma => ma.Category)
            .HasDatabaseName("IX_MediaAssets_Category");

        builder.HasIndex(ma => ma.AccessLevel)
            .HasDatabaseName("IX_MediaAssets_AccessLevel");

        builder.HasIndex(ma => ma.IsApproved)
            .HasDatabaseName("IX_MediaAssets_IsApproved");

        builder.HasIndex(ma => ma.UploadedByEmployeeId)
            .HasDatabaseName("IX_MediaAssets_UploadedByEmployeeId");

        builder.HasIndex(ma => new { ma.Type, ma.IsApproved })
            .HasDatabaseName("IX_MediaAssets_Type_IsApproved");

        builder.HasIndex(ma => new { ma.Category, ma.IsApproved })
            .HasDatabaseName("IX_MediaAssets_Category_IsApproved");

        // Índice de busca de nome (B-tree para PostgreSQL)
        builder.HasIndex(ma => ma.Name)
            .HasDatabaseName("IX_MediaAssets_Name_Search");

        // Soft delete
        builder.HasQueryFilter(ma => !ma.IsDeleted);
    }
}