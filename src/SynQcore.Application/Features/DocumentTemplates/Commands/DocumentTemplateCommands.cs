using System.ComponentModel.DataAnnotations;
using MediatR;
using SynQcore.Application.Features.DocumentTemplates.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.DocumentTemplates.Commands;

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

public class DeleteTemplateCommand : IRequest<bool>
{
    public Guid TemplateId { get; }

    public DeleteTemplateCommand(Guid templateId)
    {
        TemplateId = templateId;
    }
}

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

public class ToggleTemplateStatusCommand : IRequest<DocumentTemplateDto?>
{
    [Required]
    public Guid TemplateId { get; set; }

    public bool IsActive { get; set; }
}
