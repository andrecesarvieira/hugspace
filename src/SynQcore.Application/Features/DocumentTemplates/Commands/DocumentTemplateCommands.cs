using MediatR;
using SynQcore.Application.Features.DocumentTemplates.DTOs;
using SynQcore.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.DocumentTemplates.Commands;

/// <summary>
/// Command para criar template de documento
/// </summary>
public class CreateTemplateCommand : IRequest<DocumentTemplateDto>
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public List<TemplateFieldDto> Fields { get; set; } = new();
    public DocumentAccessLevel DefaultAccessLevel { get; set; } = DocumentAccessLevel.Internal;
    public bool RequiresApproval { get; set; }
    public List<Guid>? AllowedDepartmentIds { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Command para atualizar template de documento
/// </summary>
public class UpdateTemplateCommand : IRequest<DocumentTemplateDto?>
{
    [Required]
    public Guid Id { get; set; }

    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    public string? Content { get; set; }
    public List<TemplateFieldDto>? Fields { get; set; }
    public DocumentAccessLevel? DefaultAccessLevel { get; set; }
    public bool? RequiresApproval { get; set; }
    public List<Guid>? AllowedDepartmentIds { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Command para excluir template de documento
/// </summary>
public class DeleteTemplateCommand : IRequest<bool>
{
    public Guid TemplateId { get; }

    public DeleteTemplateCommand(Guid templateId)
    {
        TemplateId = templateId;
    }
}

/// <summary>
/// Command para criar documento a partir de template
/// </summary>
public class CreateDocumentFromTemplateCommand : IRequest<CreateDocumentFromTemplateDto?>
{
    [Required]
    public Guid TemplateId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public Dictionary<string, object> FieldValues { get; set; } = new();

    public DocumentAccessLevel? AccessLevel { get; set; }
    public Guid? DepartmentId { get; set; }
    public List<Guid>? TagIds { get; set; }
}

/// <summary>
/// Command para duplicar template
/// </summary>
public class DuplicateTemplateCommand : IRequest<DocumentTemplateDto?>
{
    [Required]
    public Guid SourceTemplateId { get; set; }

    [Required]
    [StringLength(200)]
    public string NewName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? NewDescription { get; set; }
}

/// <summary>
/// Command para alterar status do template
/// </summary>
public class ToggleTemplateStatusCommand : IRequest<DocumentTemplateDto?>
{
    [Required]
    public Guid TemplateId { get; set; }

    public bool IsActive { get; set; }
}
