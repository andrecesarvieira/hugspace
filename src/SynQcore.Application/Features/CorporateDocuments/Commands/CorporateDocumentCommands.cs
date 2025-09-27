using MediatR;
using SynQcore.Application.Features.CorporateDocuments.DTOs;
using SynQcore.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.CorporateDocuments.Commands;

/// <summary>
/// Command para criar documento corporativo
/// </summary>
public class CreateDocumentCommand : IRequest<CorporateDocumentDto>
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    public DocumentAccessLevel AccessLevel { get; set; }
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
/// Command para atualizar documento corporativo
/// </summary>
public class UpdateDocumentCommand : IRequest<CorporateDocumentDto?>
{
    [Required]
    public Guid Id { get; set; }

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
/// Command para excluir documento corporativo
/// </summary>
public class DeleteDocumentCommand : IRequest<bool>
{
    public Guid DocumentId { get; }

    public DeleteDocumentCommand(Guid documentId)
    {
        DocumentId = documentId;
    }
}

/// <summary>
/// Command para upload de nova versão do documento
/// </summary>
public class UploadDocumentVersionCommand : IRequest<CorporateDocumentDto?>
{
    [Required]
    public Guid DocumentId { get; set; }

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
/// Command para aprovar documento
/// </summary>
public class ApproveDocumentCommand : IRequest<CorporateDocumentDto?>
{
    [Required]
    public Guid DocumentId { get; set; }

    [StringLength(500)]
    public string? ApprovalNotes { get; set; }
}

/// <summary>
/// Command para rejeitar documento
/// </summary>
public class RejectDocumentCommand : IRequest<CorporateDocumentDto?>
{
    [Required]
    public Guid DocumentId { get; set; }

    [Required]
    [StringLength(500)]
    public string RejectionReason { get; set; } = string.Empty;
}

/// <summary>
/// Command para registrar acesso ao documento
/// </summary>
public class RegisterDocumentAccessCommand : IRequest<bool>
{
    [Required]
    public Guid DocumentId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public DocumentAction Action { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
