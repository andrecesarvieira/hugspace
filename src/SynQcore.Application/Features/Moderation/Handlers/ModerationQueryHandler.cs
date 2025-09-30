using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Moderation.DTOs;
using SynQcore.Application.Features.Moderation.Queries;

namespace SynQcore.Application.Features.Moderation.Handlers;

public partial class ModerationQueryHandler :
    IRequestHandler<GetModerationQueueQuery, PagedResult<ModerationDto>>,
    IRequestHandler<GetModerationByIdQuery, ModerationDto?>,
    IRequestHandler<GetModerationStatsQuery, ModerationStatsDto>,
    IRequestHandler<GetModerationCategoriesQuery, List<string>>,
    IRequestHandler<GetModerationSeveritiesQuery, List<string>>,
    IRequestHandler<GetModerationActionsQuery, List<string>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<ModerationQueryHandler> _logger;

    public ModerationQueryHandler(ISynQcoreDbContext context, ILogger<ModerationQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    // LoggerMessage delegates para performance
    [LoggerMessage(LogLevel.Information, "Processando GetModerationQueueQuery - Page: {page}, PageSize: {pageSize}")]
    private static partial void LogProcessingQueue(ILogger logger, int page, int pageSize);

    [LoggerMessage(LogLevel.Information, "Processando GetModerationByIdQuery - Id: {id}")]
    private static partial void LogProcessingById(ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Information, "Processando GetModerationStatsQuery - Período: {hours} horas")]
    private static partial void LogProcessingStats(ILogger logger, int hours);

    [LoggerMessage(LogLevel.Information, "Processando GetModerationCategoriesQuery")]
    private static partial void LogProcessingCategories(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Processando GetModerationSeveritiesQuery")]
    private static partial void LogProcessingSeverities(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Processando GetModerationActionsQuery")]
    private static partial void LogProcessingActions(ILogger logger);

    public async Task<PagedResult<ModerationDto>> Handle(GetModerationQueueQuery request, CancellationToken cancellationToken)
    {
        LogProcessingQueue(_logger, request.Page, request.PageSize);

        // Por enquanto, usar Posts como base para moderação
        var query = _context.Posts.AsQueryable();

        // Em uma implementação real, teríamos uma tabela específica de moderação

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToPaginatedResultAsync(
                request.Page,
                request.PageSize,
                p => new ModerationDto
                {
                    Id = p.Id,
                    Status = "Pending", // Status fixo por enquanto
                    CreatedAt = p.CreatedAt
                },
                cancellationToken);
    }

    public async Task<ModerationDto?> Handle(GetModerationByIdQuery request, CancellationToken cancellationToken)
    {
        LogProcessingById(_logger, request.Id);

        var post = await _context.Posts
            .Where(p => p.Id == request.Id)
            .Select(p => new ModerationDto
            {
                Id = p.Id,
                Status = "Pending",
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return post;
    }

    public async Task<ModerationStatsDto> Handle(GetModerationStatsQuery request, CancellationToken cancellationToken)
    {
        LogProcessingStats(_logger, 24);

        var cutoffDate = DateTime.UtcNow.AddHours(-24);

        // Simular estatísticas baseadas nos posts existentes
        var totalPosts = await _context.Posts.CountAsync(cancellationToken);
        var recentPosts = await _context.Posts
            .Where(p => p.CreatedAt >= cutoffDate)
            .CountAsync(cancellationToken);

        return new ModerationStatsDto
        {
            TotalPending = totalPosts / 4, // Simular 25% pendentes
            TotalApproved = totalPosts / 2, // Simular 50% aprovados
            TotalRejected = totalPosts / 4, // Simular 25% rejeitados
            TotalUnderReview = recentPosts,
            TotalEscalated = 0,
            AverageProcessingDays = 2
        };
    }

    public async Task<List<string>> Handle(GetModerationCategoriesQuery request, CancellationToken cancellationToken)
    {
        LogProcessingCategories(_logger);

        // Retornar categorias de moderação padrão
        await Task.CompletedTask; // Para manter async

        return new List<string>
        {
            "Conteúdo Inapropriado",
            "Spam",
            "Assédio",
            "Linguagem Ofensiva",
            "Desinformação",
            "Violação de Política"
        };
    }

    public async Task<List<string>> Handle(GetModerationSeveritiesQuery request, CancellationToken cancellationToken)
    {
        LogProcessingSeverities(_logger);

        await Task.CompletedTask;

        return new List<string>
        {
            "Baixa",
            "Média",
            "Alta",
            "Crítica"
        };
    }

    public async Task<List<string>> Handle(GetModerationActionsQuery request, CancellationToken cancellationToken)
    {
        LogProcessingActions(_logger);

        await Task.CompletedTask;

        return new List<string>
        {
            "Aprovado",
            "Rejeitado",
            "Removido",
            "Editado",
            "Escalado",
            "Suspenso"
        };
    }
}
