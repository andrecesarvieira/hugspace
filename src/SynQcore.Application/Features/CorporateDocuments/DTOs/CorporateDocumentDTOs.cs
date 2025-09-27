using SynQcore.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.CorporateDocuments.DTOs;

/// <summary>
/// DTO para transferência de dados de documento corporativo
/// </summary>
public class CorporateDocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
    public DocumentAccessLevel AccessLevel { get; set; }
    public bool RequiresApproval { get; set; }
    public string Version { get; set; } = "1.0";
    public long FileSizeBytes { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Tags
    public List<string> Tags { get; set; } = new();

    // Estatísticas básicas
    public int ViewCount { get; set; }
    public int DownloadCount { get; set; }
}

/// <summary>
/// DTO detalhado para documento corporativo
/// </summary>
public class CorporateDocumentDetailDto : CorporateDocumentDto
{
    public string? ApprovalNotes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public List<DocumentVersionDto> Versions { get; set; } = new();
    public List<DocumentAccessDto> RecentAccesses { get; set; } = new();
}

/// <summary>
/// DTO para versão de documento
/// </summary>
public class DocumentVersionDto
{
    public string Version { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string? VersionNotes { get; set; }
    public long FileSizeBytes { get; set; }
    public bool IsCurrent { get; set; }
}

/// <summary>
/// DTO para acesso a documento
/// </summary>
public class DocumentAccessDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DocumentAction Action { get; set; }
    public DateTime AccessedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

/// <summary>
/// DTO para estatísticas de documento
/// </summary>
public class DocumentStatsDto
{
    public Guid DocumentId { get; set; }
    public int TotalViews { get; set; }
    public int TotalDownloads { get; set; }
    public int UniqueViewers { get; set; }
    public int UniqueDownloaders { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public string? MostActiveUser { get; set; }
    public Dictionary<string, int> ViewsByDepartment { get; set; } = new();
    public Dictionary<DateTime, int> ViewsByDate { get; set; } = new();
}

/// <summary>
/// DTO para dados de arquivo
/// </summary>
public class DocumentFileDto
{
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

// Request DTOs

/// <summary>
/// Request para busca de documentos
/// </summary>
public class GetDocumentsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Title { get; set; }
    public string? Category { get; set; }
    public DocumentStatus? Status { get; set; }
    public DocumentAccessLevel? AccessLevel { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? AuthorId { get; set; }
    public List<Guid>? TagIds { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}

/// <summary>
/// Request para criação de documento
/// </summary>
public class CreateDocumentRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    public DocumentAccessLevel AccessLevel { get; set; } = DocumentAccessLevel.Internal;
    public bool RequiresApproval { get; set; }
    public Guid? DepartmentId { get; set; }
    public List<Guid>? TagIds { get; set; }

    [Required]
    public byte[] FileData { get; set; } = Array.Empty<byte>();

    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FileContentType { get; set; } = string.Empty;
}

/// <summary>
/// Request para atualização de documento
/// </summary>
public class UpdateDocumentRequest
{
    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    public DocumentAccessLevel? AccessLevel { get; set; }
    public bool? RequiresApproval { get; set; }
    public List<Guid>? TagIds { get; set; }
}

/// <summary>
/// Request para upload de nova versão
/// </summary>
public class UploadVersionRequest
{
    [StringLength(500)]
    public string? VersionNotes { get; set; }

    [Required]
    public byte[] FileData { get; set; } = Array.Empty<byte>();

    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FileContentType { get; set; } = string.Empty;
}

/// <summary>
/// Request para aprovação de documento
/// </summary>
public class ApproveDocumentRequest
{
    [StringLength(500)]
    public string? ApprovalNotes { get; set; }
}

/// <summary>
/// Request para rejeição de documento
/// </summary>
public class RejectDocumentRequest
{
    [Required]
    [StringLength(500)]
    public string RejectionReason { get; set; } = string.Empty;
}
