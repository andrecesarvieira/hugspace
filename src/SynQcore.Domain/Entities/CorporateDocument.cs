using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Documento corporativo com controle de versão e acesso granular
/// Suporta diferentes tipos de documentos empresariais com workflow de aprovação
/// </summary>
public class CorporateDocument : BaseEntity
{
    /// <summary>
    /// Título do documento corporativo
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Descrição detalhada do documento
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Nome original do arquivo quando foi enviado
    /// </summary>
    public required string OriginalFileName { get; set; }

    /// <summary>
    /// Nome do arquivo no storage (com path se necessário)
    /// </summary>
    public required string StorageFileName { get; set; }

    /// <summary>
    /// Tipo MIME do arquivo (application/pdf, image/jpeg, etc.)
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// Tamanho do arquivo em bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Tipo do documento corporativo
    /// </summary>
    public DocumentType Type { get; set; }

    /// <summary>
    /// Status atual do documento no workflow
    /// </summary>
    public DocumentStatus Status { get; set; }

    /// <summary>
    /// Nível de acesso do documento
    /// </summary>
    public DocumentAccessLevel AccessLevel { get; set; }

    /// <summary>
    /// Categoria do documento para organização
    /// </summary>
    public DocumentCategory Category { get; set; }

    /// <summary>
    /// ID do funcionário que fez upload do documento
    /// </summary>
    public Guid UploadedByEmployeeId { get; set; }

    /// <summary>
    /// ID do departamento proprietário (null = empresa toda)
    /// </summary>
    public Guid? OwnerDepartmentId { get; set; }

    /// <summary>
    /// ID do funcionário que aprovou (se necessário)
    /// </summary>
    public Guid? ApprovedByEmployeeId { get; set; }

    /// <summary>
    /// Data de aprovação do documento
    /// </summary>
    public DateTimeOffset? ApprovedAt { get; set; }

    /// <summary>
    /// Data de expiração do documento (opcional)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Tags do documento separadas por vírgula
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Número da versão atual (1.0, 1.1, 2.0, etc.)
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// ID do documento pai (para versionamento)
    /// </summary>
    public Guid? ParentDocumentId { get; set; }

    /// <summary>
    /// Indica se é a versão ativa/atual do documento
    /// </summary>
    public bool IsCurrentVersion { get; set; }

    /// <summary>
    /// Hash MD5 do arquivo para verificação de integridade
    /// </summary>
    public string? FileHash { get; set; }

    /// <summary>
    /// URL do provedor de storage externo (Azure Blob, AWS S3, etc.)
    /// </summary>
    public string? ExternalStorageUrl { get; set; }

    /// <summary>
    /// Metadados adicionais em JSON (autor original, propriedades, etc.)
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Contador de downloads/acessos
    /// </summary>
    public int DownloadCount { get; set; }

    /// <summary>
    /// Data do último acesso ao documento
    /// </summary>
    public DateTimeOffset? LastAccessedAt { get; set; }

    // Relacionamentos
    /// <summary>
    /// Funcionário que fez upload do documento
    /// </summary>
    public Employee UploadedByEmployee { get; set; } = null!;

    /// <summary>
    /// Departamento proprietário do documento
    /// </summary>
    public Department? OwnerDepartment { get; set; }

    /// <summary>
    /// Funcionário que aprovou o documento
    /// </summary>
    public Employee? ApprovedByEmployee { get; set; }

    /// <summary>
    /// Documento pai (para versionamento)
    /// </summary>
    public CorporateDocument? ParentDocument { get; set; }

    /// <summary>
    /// Versões filhas deste documento
    /// </summary>
    public ICollection<CorporateDocument> ChildVersions { get; set; } = new List<CorporateDocument>();

    /// <summary>
    /// Controle de acesso específico ao documento
    /// </summary>
    public ICollection<DocumentAccess> DocumentAccesses { get; set; } = new List<DocumentAccess>();

    /// <summary>
    /// Histórico de acessos ao documento
    /// </summary>
    public ICollection<DocumentAccessLog> AccessLogs { get; set; } = new List<DocumentAccessLog>();
}