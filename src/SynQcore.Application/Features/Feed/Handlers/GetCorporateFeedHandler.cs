using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Feed.Queries;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Features.Feed.Handlers;

/// <summary>
/// Handler para obter feed corporativo personalizado
/// Implementa algoritmo de relevância baseado em interesses e contexto corporativo
/// </summary>
public partial class GetCorporateFeedHandler : IRequestHandler<GetCorporateFeedQuery, PagedResult<FeedItemDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetCorporateFeedHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instância do GetCorporateFeedHandler
    /// </summary>
    /// <param name="context">Contexto do banco de dados</param>
    /// <param name="logger">Logger para auditoria</param>
    public GetCorporateFeedHandler(
        ISynQcoreDbContext context,
        ILogger<GetCorporateFeedHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Processa a query para obter feed corporativo personalizado
    /// </summary>
    /// <param name="request">Query de feed corporativo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado paginado do feed</returns>
    public async Task<PagedResult<FeedItemDto>> Handle(GetCorporateFeedQuery request, CancellationToken cancellationToken)
    {
        LogProcessingFeedRequest(_logger, request.UserId, request.FeedType ?? "mixed", request.PageNumber);

        // Se solicitado refresh ou feed vazio, regenera
        if (request.RefreshFeed || await ShouldRegenerateFeed(request.UserId, cancellationToken))
        {
            await RegenerateFeedForUser(request.UserId, cancellationToken);
        }

        // Busca itens do feed com paginação
        var feedQuery = BuildFeedQuery(request);

        var totalCount = await feedQuery.CountAsync(cancellationToken);

        var feedEntries = await feedQuery
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        LogRetrievedFeedItems(_logger, feedEntries.Count, request.UserId, totalCount);

        // Converte para DTOs
        var feedItems = await ConvertToFeedItems(feedEntries, request.UserId, cancellationToken);

        return new PagedResult<FeedItemDto>
        {
            Items = feedItems,
            TotalCount = totalCount,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    /// <summary>
    /// Verifica se o feed precisa ser regenerado
    /// </summary>
    private async Task<bool> ShouldRegenerateFeed(Guid userId, CancellationToken cancellationToken)
    {
        var lastFeedEntry = await _context.FeedEntries
            .Where(fe => fe.UserId == userId)
            .OrderByDescending(fe => fe.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        // Regenera se não há entradas ou se a última é muito antiga (>2 horas)
        return lastFeedEntry == null ||
               lastFeedEntry.CreatedAt < DateTime.UtcNow.AddHours(-2);
    }

    /// <summary>
    /// Constrói query base para o feed aplicando filtros
    /// </summary>
    private IQueryable<FeedEntry> BuildFeedQuery(GetCorporateFeedQuery request)
    {
        var query = _context.FeedEntries
            .Include(fe => fe.Post)
                .ThenInclude(p => p.Author)
            .Include(fe => fe.Post)
                .ThenInclude(p => p.Category)
            .Include(fe => fe.Post)
                .ThenInclude(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
            .Include(fe => fe.Department)
            .Where(fe => fe.UserId == request.UserId && !fe.IsHidden);

        // Aplica filtros se especificados
        // TODO: Implementar filtros adequadamente
        // if (request.Filters != null)
        // {
        //     query = ApplyFilters(query, request.Filters);
        // }

        // Aplica ordenação
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "date" => query.OrderByDescending(fe => fe.CreatedAt),
            "popularity" => query.OrderByDescending(fe => fe.Post.LikeCount + fe.Post.CommentCount),
            _ => query.OrderByDescending(fe => fe.Priority)
                     .ThenByDescending(fe => fe.RelevanceScore)
                     .ThenByDescending(fe => fe.CreatedAt)
        };

        return query;
    }

    /// <summary>
    /// Aplica filtros específicos à query
    /// </summary>
    private static IQueryable<FeedEntry> ApplyFilters(IQueryable<FeedEntry> query, FeedFiltersDto filters)
    {
        if (filters.PostTypes?.Count > 0)
        {
            var postTypes = filters.PostTypes.Select(pt => Enum.Parse<PostType>(pt, true));
            query = query.Where(fe => postTypes.Contains(fe.Post.Type));
        }

        if (filters.OnlyUnread == true)
        {
            query = query.Where(fe => !fe.IsRead);
        }

        if (filters.OnlyBookmarked == true)
        {
            query = query.Where(fe => fe.IsBookmarked);
        }

        if (filters.FromDate.HasValue)
        {
            var fromDate = filters.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(fe => fe.CreatedAt >= fromDate);
        }

        if (filters.ToDate.HasValue)
        {
            var toDate = filters.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(fe => fe.CreatedAt <= toDate);
        }

        if (filters.Tags?.Count > 0)
        {
            query = query.Where(fe => fe.Post.PostTags.Any(pt => filters.Tags.Contains(pt.Tag.Name)));
        }

        if (filters.Categories?.Count > 0)
        {
            query = query.Where(fe => fe.Post.Category != null && filters.Categories.Contains(fe.Post.Category.Name));
        }

        return query;
    }

    /// <summary>
    /// Regenera feed para usuário usando algoritmo de relevância
    /// </summary>
    private async Task RegenerateFeedForUser(Guid userId, CancellationToken cancellationToken)
    {
        LogRegeneratingFeed(_logger, userId);

        // Remove entradas antigas (mais de 30 dias)
        var oldEntries = _context.FeedEntries
            .Where(fe => fe.UserId == userId && fe.CreatedAt < DateTime.UtcNow.AddDays(-30));
        _context.FeedEntries.RemoveRange(oldEntries);

        // Busca posts elegíveis para o feed (últimos 30 dias, publicados)
        var eligiblePosts = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Department)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.Status == PostStatus.Published &&
                       p.CreatedAt > DateTime.UtcNow.AddDays(-30))
            .ToListAsync(cancellationToken);

        // Busca interesses do usuário para scoring
        var userInterests = await _context.UserInterests
            .Where(ui => ui.UserId == userId)
            .ToDictionaryAsync(ui => $"{ui.Type}:{ui.InterestValue}", ui => ui.Score, cancellationToken);

        // Busca informações do usuário para contexto
        var user = await _context.Employees
            .Include(e => e.EmployeeDepartments)
                .ThenInclude(ed => ed.Department)
            .FirstAsync(e => e.Id == userId, cancellationToken);

        var newFeedEntries = new List<FeedEntry>();

        foreach (var post in eligiblePosts)
        {
            // Verifica se já existe entrada para este post
            var existingEntry = await _context.FeedEntries
                .FirstOrDefaultAsync(fe => fe.UserId == userId && fe.PostId == post.Id, cancellationToken);

            if (existingEntry != null)
                continue; // Já existe no feed

            var feedEntry = CreateFeedEntry(user, post, userInterests);
            if (feedEntry != null)
            {
                newFeedEntries.Add(feedEntry);
            }
        }

        _context.FeedEntries.AddRange(newFeedEntries);
        await _context.SaveChangesAsync(cancellationToken);

        LogFeedRegenerated(_logger, userId, newFeedEntries.Count);
    }

    /// <summary>
    /// Cria entrada de feed com algoritmo de relevância
    /// </summary>
    private static FeedEntry? CreateFeedEntry(Employee user, Post post, Dictionary<string, double> userInterests)
    {
        // Não inclui posts próprios no feed
        if (post.AuthorId == user.Id)
            return null;

        var relevanceScore = CalculateRelevanceScore(user, post, userInterests);
        var priority = CalculatePriority(post, relevanceScore);
        var reason = DetermineReason(user, post, userInterests);

        return new FeedEntry
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PostId = post.Id,
            AuthorId = post.AuthorId,
            Priority = priority,
            RelevanceScore = relevanceScore,
            Reason = reason,
            DepartmentId = post.DepartmentId,
            TeamId = post.TeamId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Calcula score de relevância baseado em múltiplos fatores
    /// </summary>
    private static double CalculateRelevanceScore(Employee user, Post post, Dictionary<string, double> userInterests)
    {
        double score = 0.0;

        // Fator 1: Mesmo departamento (peso: 0.3)
        var userDepartments = user.EmployeeDepartments.Select(ed => ed.DepartmentId).ToList();
        if (post.DepartmentId.HasValue && userDepartments.Contains(post.DepartmentId.Value))
        {
            score += 0.3;
        }

        // Fator 2: Interesses em tags (peso: 0.4)
        if (post.PostTags?.Count > 0)
        {
            var tagScores = post.PostTags
                .Where(pt => userInterests.ContainsKey($"Tag:{pt.Tag.Name}"))
                .Sum(pt => userInterests[$"Tag:{pt.Tag.Name}"]);
            score += Math.Min(tagScores * 0.1, 0.4); // Máximo 0.4
        }

        // Fator 3: Tipo de post oficial (peso: 0.2)
        if (post.IsOfficial || post.Type == PostType.Announcement)
        {
            score += 0.2;
        }

        // Fator 4: Engajamento do post (peso: 0.1)
        var engagementScore = Math.Min((post.LikeCount + post.CommentCount) * 0.01, 0.1);
        score += engagementScore;

        return Math.Min(score, 1.0); // Máximo 1.0
    }

    /// <summary>
    /// Determina prioridade baseada no score e tipo de post
    /// </summary>
    private static FeedPriority CalculatePriority(Post post, double relevanceScore)
    {
        if (post.Type == PostType.Announcement && post.IsOfficial)
            return FeedPriority.Executive;

        if (post.IsPinned || relevanceScore > 0.8)
            return FeedPriority.High;

        if (relevanceScore > 0.5)
            return FeedPriority.Normal;

        return FeedPriority.Low;
    }

    /// <summary>
    /// Determina razão da inclusão no feed
    /// </summary>
    private static FeedReason DetermineReason(Employee user, Post post, Dictionary<string, double> userInterests)
    {
        if (post.IsOfficial)
            return FeedReason.Official;

        var userDepartments = user.EmployeeDepartments.Select(ed => ed.DepartmentId).ToList();
        if (post.DepartmentId.HasValue && userDepartments.Contains(post.DepartmentId.Value))
            return FeedReason.SameDepartment;

        if (post.PostTags?.Any(pt => userInterests.ContainsKey($"Tag:{pt.Tag.Name}")) == true)
            return FeedReason.TagInterest;

        return FeedReason.Recommended;
    }

    /// <summary>
    /// Converte FeedEntry para FeedItemDto com informações completas
    /// </summary>
    private async Task<List<FeedItemDto>> ConvertToFeedItems(
        List<FeedEntry> feedEntries,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var postIds = feedEntries.Select(fe => fe.PostId).ToList();

        // Busca informações de interação do usuário (comentários)
        var userComments = await _context.Comments
            .Where(c => c.AuthorId == userId && postIds.Contains(c.PostId))
            .Select(c => c.PostId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return feedEntries.ToFeedItemDtos();
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3401, Level = LogLevel.Information,
        Message = "Processing corporate feed request for user {UserId}, type: {FeedType}, page: {Page}")]
    private static partial void LogProcessingFeedRequest(ILogger logger, Guid userId, string feedType, int page);

    [LoggerMessage(EventId = 3402, Level = LogLevel.Information,
        Message = "Retrieved {ItemCount} feed items for user {UserId} (total: {TotalCount})")]
    private static partial void LogRetrievedFeedItems(ILogger logger, int itemCount, Guid userId, int totalCount);

    [LoggerMessage(EventId = 3403, Level = LogLevel.Information,
        Message = "Regenerating feed for user {UserId}")]
    private static partial void LogRegeneratingFeed(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 3404, Level = LogLevel.Information,
        Message = "Feed regenerated for user {UserId}, created {NewItemCount} new entries")]
    private static partial void LogFeedRegenerated(ILogger logger, Guid userId, int newItemCount);
}
