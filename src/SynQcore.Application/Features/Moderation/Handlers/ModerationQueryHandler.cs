using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Moderation.DTOs;
using SynQcore.Application.Features.Moderation.Queries;
using SynQcore.Application.Features.Moderation.Utilities;
using SynQcore.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SynQcore.Application.Features.Moderation.Handlers;

/// <summary>
/// Handler para processar queries relacionadas à moderação e auditoria
/// </summary>
public partial class ModerationQueryHandler : IRequestHandler<GetModerationAuditLogsQuery, PagedResult<ModerationAuditLogDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<ModerationQueryHandler> _logger;

    // LoggerMessage delegates para performance
    [LoggerMessage(LogLevel.Information, "Processando logs de auditoria - Parâmetros: Página {Page}, Tamanho {PageSize}")]
    private static partial void LogProcessingAuditLogs(ILogger logger, int page, int pageSize);

    [LoggerMessage(LogLevel.Information, "Logs de auditoria processados com sucesso - Resultados: {Count}, Página {Page} de {TotalPages}")]
    private static partial void LogAuditLogsCompleted(ILogger logger, int count, int page, int totalPages);

    public ModerationQueryHandler(ISynQcoreDbContext context, ILogger<ModerationQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<ModerationAuditLogDto>> Handle(GetModerationAuditLogsQuery request, CancellationToken cancellationToken)
    {
        LogProcessingAuditLogs(_logger, request.Page, request.PageSize);

        var query = _context.AuditLogs.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(x => x.UserId == request.UserId);
        }

        if (request.ActionType.HasValue)
            query = query.Where(x => x.ActionType == request.ActionType.Value);

        if (request.Severity.HasValue)
            query = query.Where(x => x.Severity == request.Severity.Value);

        if (request.Category.HasValue)
            query = query.Where(x => x.Category == request.Category.Value);

        if (!string.IsNullOrEmpty(request.IpAddress))
            query = query.Where(x => x.ClientIpAddress == request.IpAddress);

        if (request.StartDate.HasValue)
            query = query.Where(x => x.CreatedAt >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(x => x.CreatedAt <= request.EndDate.Value);

        if (request.OnlyRequiringAttention)
            query = query.Where(x => x.RequiresAttention == true);

        // Ordenação padrão por data descrescente
        query = query.OrderByDescending(x => x.CreatedAt);

        // Contar total de registros
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginação
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Mapear para DTOs
        var mappedItems = items.Select(ModerationMappingUtilities.MapToModerationDto).ToList();

        // Criar resultado paginado
        var result = new PagedResult<ModerationAuditLogDto>
        {
            Items = mappedItems,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };

        LogAuditLogsCompleted(_logger, result.Items.Count, result.Page, result.TotalPages);

        return result;
    }
}
