using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.DocumentTemplates.DTOs;

namespace SynQcore.Application.Features.DocumentTemplates.Queries;

/// <summary>
/// Query para buscar templates de documento com filtros
/// </summary>
public class GetTemplatesQuery : IRequest<PagedResult<DocumentTemplateDto>>
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
/// Query para buscar template específico por ID
/// </summary>
public class GetTemplateByIdQuery : IRequest<DocumentTemplateDetailDto?>
{
    public Guid TemplateId { get; }

    public GetTemplateByIdQuery(Guid templateId)
    {
        TemplateId = templateId;
    }
}

/// <summary>
/// Query para obter estatísticas de uso do template
/// </summary>
public class GetTemplateUsageStatsQuery : IRequest<TemplateUsageStatsDto?>
{
    public Guid TemplateId { get; }

    public GetTemplateUsageStatsQuery(Guid templateId)
    {
        TemplateId = templateId;
    }
}

/// <summary>
/// Query para buscar templates por categoria
/// </summary>
public class GetTemplatesByCategoryQuery : IRequest<PagedResult<DocumentTemplateDto>>
{
    public string Category { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool? IsActive { get; set; }
    public Guid? DepartmentId { get; set; }
}

/// <summary>
/// Query para buscar templates ativos
/// </summary>
public class GetActiveTemplatesQuery : IRequest<List<DocumentTemplateDto>>
{
    public Guid? DepartmentId { get; set; }
    public string? Category { get; set; }
    public int? Limit { get; set; }
}

/// <summary>
/// Query para buscar templates mais usados
/// </summary>
public class GetPopularTemplatesQuery : IRequest<List<DocumentTemplateDto>>
{
    public int Limit { get; set; } = 10;
    public string Period { get; set; } = "month"; // week, month, year
    public Guid? DepartmentId { get; set; }
    public string? Category { get; set; }
}

/// <summary>
/// Query para buscar templates criados pelo usuário
/// </summary>
public class GetMyTemplatesQuery : IRequest<PagedResult<DocumentTemplateDto>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool? IsActive { get; set; }
    public string? Category { get; set; }
}

/// <summary>
/// Query para buscar templates disponíveis para o departamento
/// </summary>
public class GetDepartmentTemplatesQuery : IRequest<List<DocumentTemplateDto>>
{
    public Guid DepartmentId { get; set; }
    public string? Category { get; set; }
    public bool ActiveOnly { get; set; } = true;
}
