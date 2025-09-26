using SynQcore.Application.Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Queries;
using SynQcore.Application.Common.DTOs;

namespace SynQcore.Application.Features.Collaboration.Handlers;

// Handler para buscar endorsements com filtros avançados
public partial class GetEndorsementsQueryHandler : IRequestHandler<GetEndorsementsQuery, PagedResult<EndorsementDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetEndorsementsQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3001, Level = LogLevel.Information,
        Message = "Buscando endorsements com filtros - Página: {Page}, Tamanho: {PageSize}")]
    private static partial void LogSearchingEndorsements(ILogger logger, int page, int pageSize);

    [LoggerMessage(EventId = 3002, Level = LogLevel.Information,
        Message = "Endorsements encontrados: {Count} | Filtros aplicados: PostId={PostId}, Type={Type}")]
    private static partial void LogEndorsementsFound(ILogger logger, int count, Guid? postId, string? type);

    [LoggerMessage(EventId = 3003, Level = LogLevel.Error,
        Message = "Erro ao buscar endorsements")]
    private static partial void LogEndorsementSearchError(ILogger logger, Exception ex);

    public GetEndorsementsQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetEndorsementsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<EndorsementDto>> Handle(GetEndorsementsQuery request, CancellationToken cancellationToken)
    {
        var search = request.SearchRequest;
        LogSearchingEndorsements(_logger, search.Page, search.PageSize);

        try
        {
            // Query base para endorsements
            var query = _context.Endorsements
                .Include(e => e.Endorser)
                .Include(e => e.Post)
                .Include(e => e.Comment)
                .AsQueryable();

            // Aplicar filtros
            if (search.PostId.HasValue)
            {
                query = query.Where(e => e.PostId == search.PostId.Value);
            }

            if (search.CommentId.HasValue)
            {
                query = query.Where(e => e.CommentId == search.CommentId.Value);
            }

            if (search.EndorserId.HasValue)
            {
                query = query.Where(e => e.EndorserId == search.EndorserId.Value);
            }

            if (search.Type.HasValue)
            {
                query = query.Where(e => e.Type == search.Type.Value);
            }

            if (!string.IsNullOrEmpty(search.Context))
            {
                query = query.Where(e => e.Context != null && e.Context.Contains(search.Context));
            }

            if (search.IsPublic.HasValue)
            {
                query = query.Where(e => e.IsPublic == search.IsPublic.Value);
            }

            if (search.EndorsedAfter.HasValue)
            {
                query = query.Where(e => e.EndorsedAt >= search.EndorsedAfter.Value);
            }

            if (search.EndorsedBefore.HasValue)
            {
                query = query.Where(e => e.EndorsedAt <= search.EndorsedBefore.Value);
            }

            // Aplicar ordenação
            var sortBy = search.SortBy ?? "EndorsedAt";
            query = sortBy.ToLowerInvariant() switch
            {
                "endorsedat" => search.SortDescending 
                    ? query.OrderByDescending(e => e.EndorsedAt) 
                    : query.OrderBy(e => e.EndorsedAt),
                "type" => search.SortDescending 
                    ? query.OrderByDescending(e => e.Type) 
                    : query.OrderBy(e => e.Type),
                "endorser" => search.SortDescending 
                    ? query.OrderByDescending(e => e.Endorser.FullName) 
                    : query.OrderBy(e => e.Endorser.FullName),
                _ => query.OrderByDescending(e => e.EndorsedAt)
            };

            // Contar total
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar paginação
            var pageSize = search.PageSize > 0 ? search.PageSize : 20;
            var page = search.Page > 0 ? search.Page : 1;
            var skip = (page - 1) * pageSize;

            var endorsements = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Mapear para DTOs usando mapeamento manual
            var endorsementDtos = endorsements.ToEndorsementDtos();

            LogEndorsementsFound(_logger, endorsementDtos.Count, search.PostId, search.Type?.ToString());

            return new PagedResult<EndorsementDto>
            {
                Items = endorsementDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        catch (Exception ex)
        {
            LogEndorsementSearchError(_logger, ex);
            throw;
        }
    }
}