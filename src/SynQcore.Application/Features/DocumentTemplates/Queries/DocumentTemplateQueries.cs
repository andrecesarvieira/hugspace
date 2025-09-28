using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.DocumentTemplates.DTOs;

namespace SynQcore.Application.Features.DocumentTemplates.Queries;

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

public class GetTemplateByIdQuery : IRequest<DocumentTemplateDetailDto?>
{
    public Guid TemplateId { get; }

    public GetTemplateByIdQuery(Guid templateId)
    {
        TemplateId = templateId;
    }
}

public class GetTemplateUsageStatsQuery : IRequest<TemplateUsageStatsDto?>
{
    public Guid TemplateId { get; }

    public GetTemplateUsageStatsQuery(Guid templateId)
    {
        TemplateId = templateId;
    }
}

public class GetTemplatesByCategoryQuery : IRequest<PagedResult<DocumentTemplateDto>>
{
    public string Category { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool? IsActive { get; set; }
    public Guid? DepartmentId { get; set; }
}

public class GetActiveTemplatesQuery : IRequest<List<DocumentTemplateDto>>
{
    public Guid? DepartmentId { get; set; }
    public string? Category { get; set; }
    public int? Limit { get; set; }
}

public class GetPopularTemplatesQuery : IRequest<List<DocumentTemplateDto>>
{
    public int Limit { get; set; } = 10;
    public string Period { get; set; } = "month"; // week, month, year
    public Guid? DepartmentId { get; set; }
    public string? Category { get; set; }
}

public class GetMyTemplatesQuery : IRequest<PagedResult<DocumentTemplateDto>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool? IsActive { get; set; }
    public string? Category { get; set; }
}

public class GetDepartmentTemplatesQuery : IRequest<List<DocumentTemplateDto>>
{
    public Guid DepartmentId { get; set; }
    public string? Category { get; set; }
    public bool ActiveOnly { get; set; } = true;
}
