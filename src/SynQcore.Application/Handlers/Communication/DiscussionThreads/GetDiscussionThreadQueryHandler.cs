using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Application.Queries.Communication.DiscussionThreads;
using AutoMapper;

namespace SynQcore.Application.Handlers.Communication.DiscussionThreads;

/// Handler para obter thread completa de discussão com analytics
public partial class GetDiscussionThreadQueryHandler : IRequestHandler<GetDiscussionThreadQuery, DiscussionThreadDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetDiscussionThreadQueryHandler> _logger;

    public GetDiscussionThreadQueryHandler(
        ISynQcoreDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ILogger<GetDiscussionThreadQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<DiscussionThreadDto> Handle(GetDiscussionThreadQuery request, CancellationToken cancellationToken)
    {
        LogRetrievingDiscussionThread(_logger, request.PostId);

        try
        {
            var currentUserId = _currentUserService.UserId;

            // Verifica se o post existe
            var post = await _context.Posts
                .Include(p => p.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

            if (post == null)
            {
                LogPostNotFound(_logger, request.PostId);
                return new DiscussionThreadDto
                {
                    PostId = request.PostId,
                    PostTitle = "Post não encontrado",
                    Comments = [],
                    TotalComments = 0
                };
            }

            // Query base para comentários
            var commentsQuery = _context.Comments
                .Include(c => c.Author)
                .Include(c => c.ResolvedBy)
                .Include(c => c.ModeratedBy)
                .Include(c => c.Mentions)
                    .ThenInclude(m => m.MentionedEmployee)
                .Include(c => c.Likes)
                .Include(c => c.Endorsements)
                .Where(c => c.PostId == request.PostId)
                .AsQueryable();

            // Aplica filtros de visibilidade baseados no usuário
            commentsQuery = ApplyVisibilityFilters(commentsQuery, currentUserId);

            // Aplica filtros específicos
            if (!request.IncludeModerated)
            {
                commentsQuery = commentsQuery.Where(c => c.ModerationStatus != Domain.Entities.Communication.ModerationStatus.Hidden &&
                                                         c.ModerationStatus != Domain.Entities.Communication.ModerationStatus.Rejected);
            }

            if (!string.IsNullOrEmpty(request.FilterByType))
            {
                if (Enum.TryParse<Domain.Entities.Communication.CommentType>(request.FilterByType, true, out var commentType))
                {
                    commentsQuery = commentsQuery.Where(c => c.Type == commentType);
                }
            }

            // Aplica ordenação
            commentsQuery = request.OrderBy.ToLowerInvariant() switch
            {
                "priority" => commentsQuery.OrderByDescending(c => c.Priority).ThenBy(c => c.CreatedAt),
                "likes" => commentsQuery.OrderByDescending(c => c.LikeCount).ThenBy(c => c.CreatedAt),
                "endorsements" => commentsQuery.OrderByDescending(c => c.EndorsementCount).ThenBy(c => c.CreatedAt),
                "lastactivity" => commentsQuery.OrderByDescending(c => c.LastActivityAt),
                "createdat" => commentsQuery.OrderBy(c => c.CreatedAt),
                _ => commentsQuery.OrderBy(c => c.ThreadPath).ThenBy(c => c.CreatedAt)
            };

            // Executa query
            var comments = await commentsQuery.ToListAsync(cancellationToken);

            // Mapeia para DTOs
            var commentDtos = comments.Select(c => MapToDiscussionCommentDto(c, currentUserId)).ToList();

            // Organiza hierarquia se ordenação por thread
            if (!string.Equals(request.OrderBy, "priority", StringComparison.OrdinalIgnoreCase) && 
                !string.Equals(request.OrderBy, "likes", StringComparison.OrdinalIgnoreCase))
            {
                commentDtos = OrganizeCommentsHierarchy(commentDtos);
            }

            // Calcula métricas da thread
            var analytics = CalculateThreadAnalytics(comments);

            LogDiscussionThreadRetrieved(_logger, request.PostId, commentDtos.Count);

            return new DiscussionThreadDto
            {
                PostId = post.Id,
                PostTitle = post.Title,
                Comments = commentDtos,
                TotalComments = analytics.TotalComments,
                UnresolvedQuestions = analytics.UnresolvedQuestions,
                FlaggedComments = analytics.FlaggedComments,
                LastActivityAt = comments.Count > 0 ? comments.Max(c => c.LastActivityAt) : post.CreatedAt
            };
        }
        catch (Exception ex)
        {
            LogErrorRetrievingThread(_logger, ex, request.PostId);
            throw;
        }
    }

    /// Aplica filtros de visibilidade baseados no usuário atual
    private static IQueryable<Domain.Entities.Communication.Comment> ApplyVisibilityFilters(
        IQueryable<Domain.Entities.Communication.Comment> query, Guid currentUserId)
    {
        return query.Where(c =>
            c.Visibility == Domain.Entities.Communication.CommentVisibility.Public ||
            (c.Visibility == Domain.Entities.Communication.CommentVisibility.Internal) ||
            (c.Visibility == Domain.Entities.Communication.CommentVisibility.Private && c.AuthorId == currentUserId) ||
            (c.Visibility == Domain.Entities.Communication.CommentVisibility.Confidential && c.AuthorId == currentUserId)
            // Aqui seria necessário verificar roles para Confidential
        );
    }

    /// Mapeia Comment para DiscussionCommentDto com dados de engagement
    private DiscussionCommentDto MapToDiscussionCommentDto(Domain.Entities.Communication.Comment comment, Guid currentUserId)
    {
        var isLikedByCurrentUser = comment.Likes.Any(l => l.EmployeeId == currentUserId);

        return new DiscussionCommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            PostId = comment.PostId,
            AuthorId = comment.AuthorId,
            AuthorName = comment.Author.FullName,
            AuthorJobTitle = comment.Author.JobTitle,
            AuthorProfilePhotoUrl = comment.Author.ProfilePhotoUrl,
            ParentCommentId = comment.ParentCommentId,
            ThreadLevel = comment.ThreadLevel,
            ThreadPath = comment.ThreadPath,
            ReplyCount = comment.ReplyCount,
            Type = comment.Type.ToString(),
            IsResolved = comment.IsResolved,
            ResolvedByName = comment.ResolvedBy?.FullName,
            ResolvedAt = comment.ResolvedAt,
            ResolutionNote = comment.ResolutionNote,
            IsEdited = comment.IsEdited,
            EditedAt = comment.EditedAt,
            IsFlagged = comment.IsFlagged,
            ModerationStatus = comment.ModerationStatus.ToString(),
            ModerationReason = comment.ModerationReason,
            ModeratedAt = comment.ModeratedAt,
            Visibility = comment.Visibility.ToString(),
            IsConfidential = comment.IsConfidential,
            IsHighlighted = comment.IsHighlighted,
            Priority = comment.Priority.ToString(),
            LikeCount = comment.LikeCount,
            EndorsementCount = comment.EndorsementCount,
            IsLikedByCurrentUser = isLikedByCurrentUser,
            LastActivityAt = comment.LastActivityAt,
            Mentions = comment.Mentions.Select(m => _mapper.Map<CommentMentionDto>(m)).ToList(),
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }

    /// Organiza comentários em hierarquia para exibição em árvore
    private static List<DiscussionCommentDto> OrganizeCommentsHierarchy(List<DiscussionCommentDto> comments)
    {
        var rootComments = comments.Where(c => !c.ParentCommentId.HasValue).ToList();
        
        foreach (var rootComment in rootComments)
        {
            PopulateReplies(rootComment, comments);
        }

        return rootComments;
    }

    /// Popula replies recursivamente
    private static void PopulateReplies(DiscussionCommentDto comment, List<DiscussionCommentDto> allComments)
    {
        var replies = allComments
            .Where(c => c.ParentCommentId == comment.Id)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        comment.Replies = replies;

        foreach (var reply in replies)
        {
            PopulateReplies(reply, allComments);
        }
    }

    /// Calcula analytics básicas da thread
    private static (int TotalComments, int UnresolvedQuestions, int FlaggedComments) CalculateThreadAnalytics(
        List<Domain.Entities.Communication.Comment> comments)
    {
        var totalComments = comments.Count;
        
        var unresolvedQuestions = comments.Count(c => 
            (c.Type == Domain.Entities.Communication.CommentType.Question || 
             c.Type == Domain.Entities.Communication.CommentType.Concern) && 
            !c.IsResolved);

        var flaggedComments = comments.Count(c => c.IsFlagged);

        return (totalComments, unresolvedQuestions, flaggedComments);
    }

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information,
        Message = "Recuperando thread de discussão: {PostId}")]
    private static partial void LogRetrievingDiscussionThread(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Warning,
        Message = "Post não encontrado: {PostId}")]
    private static partial void LogPostNotFound(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 2003, Level = LogLevel.Information,
        Message = "Thread recuperada: {PostId}, {CommentCount} comentários")]
    private static partial void LogDiscussionThreadRetrieved(ILogger logger, Guid postId, int commentCount);

    [LoggerMessage(EventId = 2004, Level = LogLevel.Error,
        Message = "Erro ao recuperar thread: {PostId}")]
    private static partial void LogErrorRetrievingThread(ILogger logger, Exception ex, Guid postId);
}