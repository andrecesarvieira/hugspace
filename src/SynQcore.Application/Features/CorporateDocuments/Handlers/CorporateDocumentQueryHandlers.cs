using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.CorporateDocuments.DTOs;
using SynQcore.Application.Features.CorporateDocuments.Queries;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.CorporateDocuments.Handlers;

internal static partial class QueryLogMessages
{
    [LoggerMessage(LogLevel.Information, "Buscando documentos corporativos - Página: {Page}, Tamanho: {PageSize}")]
    public static partial void LogSearchingDocuments(this ILogger logger, int page, int pageSize);

    [LoggerMessage(LogLevel.Information, "Buscando documento por ID: {DocumentId}")]
    public static partial void LogSearchingDocumentById(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Warning, "Documento não encontrado: {DocumentId}")]
    public static partial void LogDocumentNotFound(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Buscando documentos por categoria: {Category}")]
    public static partial void LogSearchingDocumentsByCategory(this ILogger logger, string category);

    [LoggerMessage(LogLevel.Warning, "Categoria inválida: {Category}")]
    public static partial void LogInvalidCategory(this ILogger logger, string category);
}

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, PagedResult<CorporateDocumentDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetDocumentsQueryHandler> _logger;

    public GetDocumentsQueryHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<GetDocumentsQueryHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PagedResult<CorporateDocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogSearchingDocuments(request.Page, request.PageSize);

        var query = _context.CorporateDocuments
            .Include(d => d.UploadedByEmployee)
            .Include(d => d.OwnerDepartment)
            .Include(d => d.ApprovedByEmployee)
            .AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            query = query.Where(d =>
                d.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (d.Description != null && d.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (d.Tags != null && d.Tags.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }
        if (!string.IsNullOrEmpty(request.Category))
        {
            if (Enum.TryParse<DocumentCategory>(request.Category, out var category))
            {
                query = query.Where(d => d.Category == category);
            }
        }

        if (request.Status.HasValue)
        {
            query = query.Where(d => d.Status == request.Status.Value);
        }

        if (request.AccessLevel.HasValue)
        {
            query = query.Where(d => d.AccessLevel == request.AccessLevel.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(d => d.OwnerDepartmentId == request.DepartmentId.Value);
        }

        if (request.AuthorId.HasValue)
        {
            query = query.Where(d => d.UploadedByEmployeeId == request.AuthorId.Value);
        }

        // Filtrar por data se especificada
        if (request.CreatedAfter.HasValue)
        {
            query = query.Where(d => d.CreatedAt >= request.CreatedAfter.Value);
        }

        if (request.CreatedBefore.HasValue)
        {
            query = query.Where(d => d.CreatedAt <= request.CreatedBefore.Value);
        }

        // Ordenação
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "title" => request.SortDirection == "desc" ?
                query.OrderByDescending(d => d.Title) :
                query.OrderBy(d => d.Title),
            "createdat" => request.SortDirection == "desc" ?
                query.OrderByDescending(d => d.CreatedAt) :
                query.OrderBy(d => d.CreatedAt),
            "updatedat" => request.SortDirection == "desc" ?
                query.OrderByDescending(d => d.UpdatedAt) :
                query.OrderBy(d => d.UpdatedAt),
            "author" => request.SortDirection == "desc" ?
                query.OrderByDescending(d => d.UploadedByEmployee.FullName) :
                query.OrderBy(d => d.UploadedByEmployee.FullName),
            _ => query.OrderByDescending(d => d.CreatedAt)
        };

        // Paginação
        var totalCount = await query.CountAsync(cancellationToken);
        var documents = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Mapear para DTOs usando método de extensão
        var documentDtos = documents.ToCorporateDocumentDtos();

        return new PagedResult<CorporateDocumentDto>
        {
            Items = documentDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}

public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, CorporateDocumentDetailDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetDocumentByIdQueryHandler> _logger;

    public GetDocumentByIdQueryHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<GetDocumentByIdQueryHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CorporateDocumentDetailDto?> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogSearchingDocumentById(request.DocumentId);

        var document = await _context.CorporateDocuments
            .Include(d => d.UploadedByEmployee)
            .Include(d => d.OwnerDepartment)
            .Include(d => d.ApprovedByEmployee)
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

        if (document == null)
        {
            _logger.LogDocumentNotFound(request.DocumentId);
            return null;
        }

        // Mapear para DTO detalhado usando método de extensão
        var documentDto = document.ToCorporateDocumentDetailDto();

        return documentDto;
    }
}

public class GetDocumentsByCategoryQueryHandler : IRequestHandler<GetDocumentsByCategoryQuery, PagedResult<CorporateDocumentDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetDocumentsByCategoryQueryHandler> _logger;

    public GetDocumentsByCategoryQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetDocumentsByCategoryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<CorporateDocumentDto>> Handle(GetDocumentsByCategoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogSearchingDocumentsByCategory(request.Category);

        if (!Enum.TryParse<DocumentCategory>(request.Category, out var category))
        {
            _logger.LogInvalidCategory(request.Category);
            return new PagedResult<CorporateDocumentDto>
            {
                Items = new List<CorporateDocumentDto>(),
                TotalCount = 0,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = 0
            };
        }

        var query = _context.CorporateDocuments
            .Include(d => d.UploadedByEmployee)
            .Include(d => d.OwnerDepartment)
            .Include(d => d.ApprovedByEmployee)
            .Where(d => d.Category == category);

        if (request.AccessLevel.HasValue)
        {
            query = query.Where(d => d.AccessLevel == request.AccessLevel.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(d => d.OwnerDepartmentId == request.DepartmentId.Value);
        }

        query = query.OrderByDescending(d => d.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var documents = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Mapear para DTOs usando método de extensão
        var documentDtos = documents.ToCorporateDocumentDtos();

        return new PagedResult<CorporateDocumentDto>
        {
            Items = documentDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}

