using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.DocumentTemplates.DTOs;
using SynQcore.Application.Features.DocumentTemplates.Queries;

namespace SynQcore.Application.Features.DocumentTemplates.Handlers;

public class GetTemplatesQueryHandler : IRequestHandler<GetTemplatesQuery, PagedResult<DocumentTemplateDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetTemplatesQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, int, int, Exception?> LogBuscandoTemplates =
        LoggerMessage.Define<int, int>(LogLevel.Information, new EventId(5401, nameof(LogBuscandoTemplates)),
            "Buscando templates de documento - Página: {Page}, Tamanho: {PageSize}");

    private static readonly Action<ILogger, int, Exception?> LogTemplatesEncontrados =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5402, nameof(LogTemplatesEncontrados)),
            "Templates de documento encontrados: {Count} resultados");

    private static readonly Action<ILogger, Exception?> LogErroTemplates =
        LoggerMessage.Define(LogLevel.Error, new EventId(5499, nameof(LogErroTemplates)),
            "Erro ao buscar templates de documento");

    public GetTemplatesQueryHandler(ISynQcoreDbContext context, ILogger<GetTemplatesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<DocumentTemplateDto>> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoTemplates(_logger, request.Page, request.PageSize, null);

        try
        {
            var query = _context.DocumentTemplates
                .Include(t => t.CreatedByEmployee)
                .Where(t => !t.IsDeleted);

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(t => t.Name.Contains(request.Name));
            }

            if (!string.IsNullOrEmpty(request.Category))
            {
                query = query.Where(t => t.DefaultCategory.ToString().Contains(request.Category));
            }

            if (request.DepartmentId.HasValue)
            {
                // Filtrar por departamento do criador
                query = query.Where(t => t.CreatedByEmployee != null &&
                    t.CreatedByEmployee.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive);
            }

            // Filtros de data e tags podem ser implementados quando necessário

            // Aplicar ordenação
            query = request.SortBy?.ToLower(CultureInfo.InvariantCulture) switch
            {
                "name" => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc"
                    ? query.OrderBy(t => t.Name)
                    : query.OrderByDescending(t => t.Name),
                "category" => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc"
                    ? query.OrderBy(t => t.DefaultCategory)
                    : query.OrderByDescending(t => t.DefaultCategory),
                "usage" => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc"
                    ? query.OrderBy(t => t.UsageCount)
                    : query.OrderByDescending(t => t.UsageCount),
                "version" => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc"
                    ? query.OrderBy(t => t.Version)
                    : query.OrderByDescending(t => t.Version),
                _ => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc"
                    ? query.OrderBy(t => t.CreatedAt)
                    : query.OrderByDescending(t => t.CreatedAt)
            };

            // Contar total de registros
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar paginação
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Mapear para DTOs
            var itemDtos = items.Select(t => t.ToDocumentTemplateDto()).ToList();

            LogTemplatesEncontrados(_logger, itemDtos.Count, null);

            return new PagedResult<DocumentTemplateDto>
            {
                Items = itemDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
        }
        catch (Exception ex)
        {
            LogErroTemplates(_logger, ex);
            throw;
        }
    }
}
