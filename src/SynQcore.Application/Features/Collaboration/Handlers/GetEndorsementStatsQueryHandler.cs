using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Queries;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.Handlers;

/// <summary>
/// Handler para obter estatísticas detalhadas de endorsements de conteúdo
/// </summary>
public partial class GetEndorsementStatsQueryHandler : IRequestHandler<GetEndorsementStatsQuery, EndorsementStatsDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetEndorsementStatsQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3051, Level = LogLevel.Information,
        Message = "Calculando estatísticas de endorsements para {ContentType}:{ContentId}")]
    private static partial void LogCalculatingStats(ILogger logger, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3052, Level = LogLevel.Information,
        Message = "Estatísticas calculadas: {TotalEndorsements} endorsements para {ContentType}:{ContentId}")]
    private static partial void LogStatsCalculated(ILogger logger, int totalEndorsements, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3053, Level = LogLevel.Warning,
        Message = "Conteúdo não encontrado para stats - {ContentType}:{ContentId}")]
    private static partial void LogStatsContentNotFound(ILogger logger, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3054, Level = LogLevel.Error,
        Message = "Erro ao calcular estatísticas para {ContentType}:{ContentId}")]
    private static partial void LogStatsError(ILogger logger, string contentType, Guid contentId, Exception ex);

    public GetEndorsementStatsQueryHandler(
        ISynQcoreDbContext context, 
        ILogger<GetEndorsementStatsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EndorsementStatsDto> Handle(GetEndorsementStatsQuery request, CancellationToken cancellationToken)
    {
        var contentType = request.PostId.HasValue ? "Post" : "Comment";
        var contentId = request.PostId ?? request.CommentId!.Value;

        LogCalculatingStats(_logger, contentType, contentId);

        try
        {
            // Validar se o conteúdo existe
            bool contentExists;
            if (request.PostId.HasValue)
            {
                contentExists = await _context.Posts.AnyAsync(p => p.Id == request.PostId.Value, cancellationToken);
            }
            else
            {
                contentExists = await _context.Comments.AnyAsync(c => c.Id == request.CommentId!.Value, cancellationToken);
            }

            if (!contentExists)
            {
                LogStatsContentNotFound(_logger, contentType, contentId);
                throw new ArgumentException($"{contentType} com ID {contentId} não encontrado.");
            }

            // Buscar endorsements do conteúdo
            var endorsementsQuery = _context.Endorsements.AsQueryable();

            if (request.PostId.HasValue)
            {
                endorsementsQuery = endorsementsQuery.Where(e => e.PostId == request.PostId.Value);
            }
            else
            {
                endorsementsQuery = endorsementsQuery.Where(e => e.CommentId == request.CommentId!.Value);
            }

            if (!request.IncludePrivate)
            {
                endorsementsQuery = endorsementsQuery.Where(e => e.IsPublic);
            }

            // Calcular estatísticas por tipo
            var stats = await endorsementsQuery
                .GroupBy(e => e.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var totalEndorsements = stats.Sum(s => s.Count);

            // Montar DTO de estatísticas
            var result = new EndorsementStatsDto
            {
                ContentId = contentId,
                ContentType = contentType,
                TotalEndorsements = totalEndorsements,
                HelpfulCount = stats.FirstOrDefault(s => s.Type == EndorsementType.Helpful)?.Count ?? 0,
                InsightfulCount = stats.FirstOrDefault(s => s.Type == EndorsementType.Insightful)?.Count ?? 0,
                AccurateCount = stats.FirstOrDefault(s => s.Type == EndorsementType.Accurate)?.Count ?? 0,
                InnovativeCount = stats.FirstOrDefault(s => s.Type == EndorsementType.Innovative)?.Count ?? 0,
                ComprehensiveCount = stats.FirstOrDefault(s => s.Type == EndorsementType.Comprehensive)?.Count ?? 0,
                WellResearchedCount = stats.FirstOrDefault(s => s.Type == EndorsementType.WellResearched)?.Count ?? 0,
                ActionableCount = stats.FirstOrDefault(s => s.Type == EndorsementType.Actionable)?.Count ?? 0,
                StrategicCount = stats.FirstOrDefault(s => s.Type == EndorsementType.Strategic)?.Count ?? 0
            };

            // Determinar tipo de endorsement mais popular
            if (totalEndorsements > 0)
            {
                var topStat = stats.OrderByDescending(s => s.Count).First();
                result.TopEndorsementType = topStat.Type;
                result.TopEndorsementTypeIcon = GetEndorsementTypeIcon(topStat.Type);
            }

            LogStatsCalculated(_logger, totalEndorsements, contentType, contentId);
            return result;
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            LogStatsError(_logger, contentType, contentId, ex);
            throw;
        }
    }

    /// <summary>
    /// Obter ícone para tipo de endorsement
    /// </summary>
    private static string GetEndorsementTypeIcon(EndorsementType type) => type switch
    {
        EndorsementType.Helpful => "🔥",
        EndorsementType.Insightful => "💡",
        EndorsementType.Accurate => "✅",
        EndorsementType.Innovative => "🚀",
        EndorsementType.Comprehensive => "📚",
        EndorsementType.WellResearched => "🔍",
        EndorsementType.Actionable => "⚡",
        EndorsementType.Strategic => "🎯",
        _ => "👍"
    };
}