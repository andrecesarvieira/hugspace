using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Template de documento corporativo para padronização
/// Define estruturas reutilizáveis para diferentes tipos de documentos
/// </summary>
public class DocumentTemplate : BaseEntity
{
    /// <summary>
    /// Nome do template
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Descrição do template e seu uso
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tipo de documento que este template representa
    /// </summary>
    public DocumentType DocumentType { get; set; }

    /// <summary>
    /// Categoria padrão para documentos criados com este template
    /// </summary>
    public DocumentCategory DefaultCategory { get; set; }

    /// <summary>
    /// Nível de acesso padrão para documentos deste template
    /// </summary>
    public DocumentAccessLevel DefaultAccessLevel { get; set; }

    /// <summary>
    /// Nome do arquivo template armazenado
    /// </summary>
    public required string TemplateFileName { get; set; }

    /// <summary>
    /// Tipo MIME do arquivo template
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// Tamanho do arquivo template em bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// URL do template no storage
    /// </summary>
    public string? StorageUrl { get; set; }

    /// <summary>
    /// Versão do template (1.0, 1.1, etc.)
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// Indica se o template está ativo e disponível para uso
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indica se é o template padrão para este tipo de documento
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Placeholders disponíveis no template (JSON)
    /// Ex: {"companyName": "Nome da Empresa", "date": "Data Atual"}
    /// </summary>
    public string? Placeholders { get; set; }

    /// <summary>
    /// Instruções de uso do template
    /// </summary>
    public string? UsageInstructions { get; set; }

    /// <summary>
    /// ID do departamento responsável pelo template (null = global)
    /// </summary>
    public Guid? OwnerDepartmentId { get; set; }

    /// <summary>
    /// ID do funcionário que criou o template
    /// </summary>
    public Guid CreatedByEmployeeId { get; set; }

    /// <summary>
    /// Tags do template separadas por vírgula
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Contador de quantas vezes o template foi usado
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Data da última utilização do template
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    // Relacionamentos
    /// <summary>
    /// Departamento responsável pelo template
    /// </summary>
    public Department? OwnerDepartment { get; set; }

    /// <summary>
    /// Funcionário que criou o template
    /// </summary>
    public Employee CreatedByEmployee { get; set; } = null!;
}

/// <summary>
/// Asset de mídia corporativa (logos, imagens, vídeos, etc.)
/// </summary>
public class MediaAsset : BaseEntity
{
    /// <summary>
    /// Nome do asset
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Descrição do asset e seu uso
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tipo de asset de mídia
    /// </summary>
    public MediaAssetType Type { get; set; }

    /// <summary>
    /// Categoria do asset para organização
    /// </summary>
    public MediaAssetCategory Category { get; set; }

    /// <summary>
    /// Nome original do arquivo
    /// </summary>
    public required string OriginalFileName { get; set; }

    /// <summary>
    /// Nome do arquivo no storage
    /// </summary>
    public required string StorageFileName { get; set; }

    /// <summary>
    /// Tipo MIME do arquivo
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// Tamanho do arquivo em bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Largura da imagem/vídeo (pixels)
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Altura da imagem/vídeo (pixels)
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Duração do vídeo/áudio (segundos)
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// URL do asset no storage
    /// </summary>
    public string? StorageUrl { get; set; }

    /// <summary>
    /// URL da thumbnail (para vídeos/imagens)
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Nível de acesso do asset
    /// </summary>
    public DocumentAccessLevel AccessLevel { get; set; }

    /// <summary>
    /// Indica se é um asset aprovado para uso corporativo
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// ID do funcionário que fez upload
    /// </summary>
    public Guid UploadedByEmployeeId { get; set; }

    /// <summary>
    /// ID do funcionário que aprovou
    /// </summary>
    public Guid? ApprovedByEmployeeId { get; set; }

    /// <summary>
    /// Data de aprovação
    /// </summary>
    public DateTimeOffset? ApprovedAt { get; set; }

    /// <summary>
    /// Tags do asset separadas por vírgula
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Metadados adicionais (EXIF, etc.) em JSON
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Contador de downloads/usos
    /// </summary>
    public int DownloadCount { get; set; }

    // Relacionamentos
    /// <summary>
    /// Funcionário que fez upload
    /// </summary>
    public Employee UploadedByEmployee { get; set; } = null!;

    /// <summary>
    /// Funcionário que aprovou
    /// </summary>
    public Employee? ApprovedByEmployee { get; set; }
}