using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Application.Features.MediaAssets.Queries;
using SynQcore.Application.Common.Extensions;
using System.Globalization;

namespace SynQcore.Application.Features.MediaAssets.Handlers;

/// <summary>
/// Handler para buscar assets de mídia com filtros avançados
/// </summary>
public class GetMediaAssetsQueryHandler : IRequestHandler<GetMediaAssetsQuery, PagedResult<MediaAssetDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetMediaAssetsQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, int, int, Exception?> LogBuscandoMediaAssets =
        LoggerMessage.Define<int, int>(LogLevel.Information, new EventId(5301, nameof(LogBuscandoMediaAssets)),
            "Buscando assets de mídia - Página: {Page}, Tamanho: {PageSize}");

    private static readonly Action<ILogger, int, Exception?> LogMediaAssetsEncontrados =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5302, nameof(LogMediaAssetsEncontrados)),
            "Assets de mídia encontrados: {Count} resultados");

    private static readonly Action<ILogger, Exception?> LogErroMediaAssets =
        LoggerMessage.Define(LogLevel.Error, new EventId(5399, nameof(LogErroMediaAssets)),
            "Erro ao buscar assets de mídia");

    public GetMediaAssetsQueryHandler(ISynQcoreDbContext context, ILogger<GetMediaAssetsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<MediaAssetDto>> Handle(GetMediaAssetsQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoMediaAssets(_logger, request.Page, request.PageSize, null);

        try
        {
            var query = _context.MediaAssets
                .Include(m => m.UploadedByEmployee)
                .Include(m => m.ApprovedByEmployee)
                .Where(m => !m.IsDeleted);

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.Title))
            {
                query = query.Where(m => m.Name.Contains(request.Title));
            }

            if (!string.IsNullOrEmpty(request.AssetType))
            {
                query = query.Where(m => m.Type.ToString().Contains(request.AssetType));
            }

            if (request.AccessLevel.HasValue)
            {
                query = query.Where(m => m.AccessLevel == request.AccessLevel);
            }

            if (request.CreatedAfter.HasValue)
            {
                query = query.Where(m => m.CreatedAt >= request.CreatedAfter);
            }

            if (request.CreatedBefore.HasValue)
            {
                query = query.Where(m => m.CreatedAt <= request.CreatedBefore);
            }

            if (request.DepartmentId.HasValue)
            {
                // Filtrar por departamento do autor
                query = query.Where(m => m.UploadedByEmployee != null && 
                    m.UploadedByEmployee.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId));
            }

            if (request.AuthorId.HasValue)
            {
                query = query.Where(m => m.UploadedByEmployeeId == request.AuthorId);
            }

            if (request.TagIds != null && request.TagIds.Count > 0)
            {
                var tagNames = string.Join(",", request.TagIds);
                query = query.Where(m => m.Tags != null && m.Tags.Contains(tagNames));
            }

            // Aplicar ordenação
            query = request.SortBy?.ToLower(CultureInfo.InvariantCulture) switch
            {
                "name" => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc" 
                    ? query.OrderBy(m => m.Name) 
                    : query.OrderByDescending(m => m.Name),
                "type" => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc" 
                    ? query.OrderBy(m => m.Type) 
                    : query.OrderByDescending(m => m.Type),
                "size" => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc" 
                    ? query.OrderBy(m => m.FileSizeBytes) 
                    : query.OrderByDescending(m => m.FileSizeBytes),
                "downloads" => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc" 
                    ? query.OrderBy(m => m.DownloadCount) 
                    : query.OrderByDescending(m => m.DownloadCount),
                _ => request.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "asc" 
                    ? query.OrderBy(m => m.CreatedAt) 
                    : query.OrderByDescending(m => m.CreatedAt)
            };

            // Contar total de registros
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar paginação
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Mapear para DTOs
            var itemDtos = items.Select(m => m.ToMediaAssetDto()).ToList();

            LogMediaAssetsEncontrados(_logger, itemDtos.Count, null);

            return new PagedResult<MediaAssetDto>
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
            LogErroMediaAssets(_logger, ex);
            throw;
        }
    }
}