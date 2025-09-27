using SynQcore.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.DocumentTemplates.DTOs;

/// <summary>
/// DTO para transferência de dados de template de documento
/// </summary>
public class DocumentTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public DocumentAccessLevel DefaultAccessLevel { get; set; }
    public bool RequiresApproval { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public List<string> AllowedDepartments { get; set; } = new();

    // Estatísticas básicas
    public int UsageCount { get; set; }
    public DateTime? LastUsedAt { get; set; }
}

/// <summary>
/// DTO detalhado para template de documento
/// </summary>
public class DocumentTemplateDetailDto : DocumentTemplateDto
{
    public string Content { get; set; } = string.Empty;
    public List<TemplateFieldDto> Fields { get; set; } = new();
    public List<TemplateUsageDto> RecentUsages { get; set; } = new();
}

/// <summary>
/// DTO para campo de template
/// </summary>
public class TemplateFieldDto
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // text, number, date, select, etc.
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; }
    public List<string>? Options { get; set; } // Para campos select
}

/// <summary>
/// DTO para uso do template
/// </summary>
public class TemplateUsageDto
{
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public Guid UsedByUserId { get; set; }
    public string UsedByUserName { get; set; } = string.Empty;
    public DateTime UsedAt { get; set; }
}

/// <summary>
/// DTO para estatísticas de uso do template
/// </summary>
public class TemplateUsageStatsDto
{
    public Guid TemplateId { get; set; }
    public int TotalUsages { get; set; }
    public int UniqueUsers { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string? MostActiveUser { get; set; }
    public Dictionary<string, int> UsagesByDepartment { get; set; } = new();
    public Dictionary<DateTime, int> UsagesByDate { get; set; } = new();
}

/// <summary>
/// DTO para resultado de criação de documento a partir de template
/// </summary>
public class CreateDocumentFromTemplateDto
{
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public string GeneratedContent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Request DTOs

/// <summary>
/// Request para busca de templates
/// </summary>
public class GetTemplatesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Name { get; set; }
    public string? Category { get; set; }
    public bool? IsActive { get; set; }
    public Guid? DepartmentId { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}

/// <summary>
/// Request para criação de template
/// </summary>
public class CreateTemplateRequest
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
/// Request para atualização de template
/// </summary>
public class UpdateTemplateRequest
{
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
/// Request para criação de documento a partir de template
/// </summary>
public class CreateDocumentFromTemplateRequest
{
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
/// Request para duplicação de template
/// </summary>
public class DuplicateTemplateRequest
{
    [Required]
    [StringLength(200)]
    public string NewName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? NewDescription { get; set; }
}

/// <summary>
/// Request para alteração de status do template
/// </summary>
public class ToggleTemplateStatusRequest
{
    public bool IsActive { get; set; }
}
