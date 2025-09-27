using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.CorporateDocuments.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.CorporateDocuments.Queries;

/// <summary>
/// Query para buscar documentos corporativos com filtros
/// </summary>
public class GetDocumentsQuery : IRequest<PagedResult<CorporateDocumentDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public DocumentStatus? Status { get; set; }
    public DocumentAccessLevel? AccessLevel { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? AuthorId { get; set; }
    public List<Guid>? TagIds { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}

/// <summary>
/// Query para buscar documento específico por ID
/// </summary>
public class GetDocumentByIdQuery : IRequest<CorporateDocumentDetailDto?>
{
    public Guid DocumentId { get; }

    public GetDocumentByIdQuery(Guid documentId)
    {
        DocumentId = documentId;
    }
}

/// <summary>
/// Query para obter arquivo do documento
/// </summary>
public class GetDocumentFileQuery : IRequest<DocumentFileDto?>
{
    public Guid DocumentId { get; }

    public GetDocumentFileQuery(Guid documentId)
    {
        DocumentId = documentId;
    }
}

/// <summary>
/// Query para obter histórico de acessos do documento
/// </summary>
public class GetDocumentAccessHistoryQuery : IRequest<PagedResult<DocumentAccessDto>>
{
    public Guid DocumentId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Query para obter estatísticas de documento
/// </summary>
public class GetDocumentStatsQuery : IRequest<DocumentStatsDto?>
{
    public Guid DocumentId { get; }

    public GetDocumentStatsQuery(Guid documentId)
    {
        DocumentId = documentId;
    }
}

/// <summary>
/// Query para buscar documentos por categoria
/// </summary>
public class GetDocumentsByCategoryQuery : IRequest<PagedResult<CorporateDocumentDto>>
{
    public string Category { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DocumentAccessLevel? AccessLevel { get; set; }
    public Guid? DepartmentId { get; set; }
}

/// <summary>
/// Query para buscar documentos pendentes de aprovação
/// </summary>
public class GetPendingApprovalsQuery : IRequest<PagedResult<CorporateDocumentDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? DepartmentId { get; set; }
    public string? Category { get; set; }
}

/// <summary>
/// Query para buscar documentos recentes
/// </summary>
public class GetRecentDocumentsQuery : IRequest<List<CorporateDocumentDto>>
{
    public int Limit { get; set; } = 10;
    public Guid? DepartmentId { get; set; }
    public DocumentAccessLevel? MaxAccessLevel { get; set; }
}

/// <summary>
/// Query para buscar documentos populares
/// </summary>
public class GetPopularDocumentsQuery : IRequest<List<CorporateDocumentDto>>
{
    public int Limit { get; set; } = 10;
    public string Period { get; set; } = "month"; // week, month, year
    public Guid? DepartmentId { get; set; }
    public string? Category { get; set; }
}

/// <summary>
/// Query para buscar documentos do usuário
/// </summary>
public class GetMyDocumentsQuery : IRequest<PagedResult<CorporateDocumentDto>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DocumentStatus? Status { get; set; }
    public string? Category { get; set; }
}

/// <summary>
/// Query para buscar versões de documento
/// </summary>
public class GetDocumentVersionsQuery : IRequest<List<DocumentVersionDto>>
{
    public Guid DocumentId { get; }

    public GetDocumentVersionsQuery(Guid documentId)
    {
        DocumentId = documentId;
    }
}
