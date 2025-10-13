using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Feed.Commands;
using SynQcore.Application.Features.Feed.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Feed.Handlers;

/// <summary>
/// Handler para atualização de posts no feed corporativo
/// </summary>
public partial class UpdateFeedPostHandler : IRequestHandler<UpdateFeedPostCommand, FeedPostDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UpdateFeedPostHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IFeedPostCacheService _cacheService;

    public UpdateFeedPostHandler(
        ISynQcoreDbContext context,
        ILogger<UpdateFeedPostHandler> logger,
        IMediator mediator,
        IFeedPostCacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _mediator = mediator;
        _cacheService = cacheService;
    }

    public async Task<FeedPostDto> Handle(UpdateFeedPostCommand request, CancellationToken cancellationToken)
    {
        LogUpdatingFeedPost(_logger, request.PostId, request.UserId);

        try
        {
            // Buscar o post com relacionamentos
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Author.EmployeeDepartments).ThenInclude(ed => ed.Department)
                .FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);

            if (post == null)
            {
                LogPostNotFound(_logger, request.PostId);
                throw new InvalidOperationException("Post não encontrado");
            }

            // Verificar se o usuário pode editar (apenas o autor)
            if (post.AuthorId != request.UserId)
            {
                LogUnauthorizedUpdate(_logger, request.PostId, request.UserId, post.AuthorId);
                throw new UnauthorizedAccessException("Apenas o autor pode editar este post");
            }

            // Atualizar propriedades se fornecidas
            if (!string.IsNullOrWhiteSpace(request.Content))
            {
                post.Content = request.Content;
                post.Summary = request.Content.Length > 100 ? request.Content[..100] + "..." : request.Content;
            }

            if (request.ImageUrl != null)
                post.ImageUrl = request.ImageUrl;

            if (request.IsPublic.HasValue)
                post.Visibility = request.IsPublic.Value ? PostVisibility.Company : PostVisibility.Department;

            post.UpdatedAt = DateTime.UtcNow;

            // Atualizar tags se fornecidas
            if (request.Tags != null)
            {
                // Remover tags existentes
                var existingPostTags = post.PostTags.ToList();
                foreach (var postTag in existingPostTags)
                {
                    _context.PostTags.Remove(postTag);
                }

                // Adicionar novas tags
                if (request.Tags.Length > 0)
                {
                    foreach (var tagName in request.Tags.Where(t => !string.IsNullOrWhiteSpace(t)))
                    {
                        // Buscar ou criar tag
                        var existingTag = await _context.Tags
                            .FirstOrDefaultAsync(t => t.Name == tagName.Trim(), cancellationToken);

                        if (existingTag == null)
                        {
                            var newTag = new Tag
                            {
                                Id = Guid.NewGuid(),
                                Name = tagName.Trim(),
                                Description = $"Tag criada automaticamente: {tagName.Trim()}",
                                Color = "#3B82F6",
                                CreatedAt = DateTime.UtcNow
                            };
                            _context.Tags.Add(newTag);
                            existingTag = newTag;
                        }

                        // Criar associação post-tag
                        var postTag = new PostTag
                        {
                            PostId = post.Id,
                            TagId = existingTag.Id
                        };
                        _context.PostTags.Add(postTag);
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            LogFeedPostUpdated(_logger, post.Id, request.UserId);

            // Invalidar caches relacionados
            await _cacheService.RemovePostAsync(post.Id, cancellationToken);
            await _cacheService.InvalidateUserFeedAsync(request.UserId, cancellationToken);
            await _cacheService.InvalidateAllPostCachesAsync(cancellationToken);

            // Forçar regeneração do feed para refletir alterações
            try
            {
                var regenerateCommand = new RegenerateFeedCommand
                {
                    UserId = request.UserId,
                    PreserveBookmarks = true,
                    DaysToInclude = 30
                };
                await _mediator.Send(regenerateCommand, cancellationToken);
            }
            catch (Exception ex)
            {
                LogFeedRegenerationError(_logger, ex, request.UserId);
                // Não falha a atualização por erro na regeneração do feed
            }

            // Retornar DTO atualizado
            return new FeedPostDto
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                Tags = post.PostTags.Select(pt => pt.Tag.Name).ToList(),
                IsPublic = post.Visibility == PostVisibility.Company,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.FullName,
                AuthorAvatarUrl = post.Author.ProfilePhotoUrl,
                AuthorDepartment = post.Author.EmployeeDepartments.FirstOrDefault()?.Department?.Name,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                ViewCount = post.ViewCount,
                Status = post.Status.ToString(),
                Type = "FeedPost",
                Visibility = post.Visibility.ToString()
            };
        }
        catch (Exception ex)
        {
            LogErrorUpdatingFeedPost(_logger, ex, request.PostId, request.UserId);
            throw;
        }
    }

    [LoggerMessage(EventId = 7101, Level = LogLevel.Information,
        Message = "Atualizando post no feed - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogUpdatingFeedPost(ILogger logger, Guid postId, Guid userId);

    [LoggerMessage(EventId = 7102, Level = LogLevel.Warning,
        Message = "Post não encontrado: {PostId}")]
    private static partial void LogPostNotFound(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 7103, Level = LogLevel.Warning,
        Message = "Tentativa não autorizada de editar post - PostId: {PostId}, UserId: {UserId}, AuthorId: {AuthorId}")]
    private static partial void LogUnauthorizedUpdate(ILogger logger, Guid postId, Guid userId, Guid authorId);

    [LoggerMessage(EventId = 7104, Level = LogLevel.Information,
        Message = "Post do feed atualizado com sucesso - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogFeedPostUpdated(ILogger logger, Guid postId, Guid userId);

    [LoggerMessage(EventId = 7105, Level = LogLevel.Warning,
        Message = "Erro ao regenerar feed após atualização - UserId: {UserId}")]
    private static partial void LogFeedRegenerationError(ILogger logger, Exception ex, Guid userId);

    [LoggerMessage(EventId = 7106, Level = LogLevel.Error,
        Message = "Erro ao atualizar post no feed - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogErrorUpdatingFeedPost(ILogger logger, Exception ex, Guid postId, Guid userId);
}

/// <summary>
/// Handler para exclusão de posts no feed corporativo
/// </summary>
public partial class DeleteFeedPostHandler : IRequestHandler<DeleteFeedPostCommand>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<DeleteFeedPostHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IFeedPostCacheService _cacheService;

    public DeleteFeedPostHandler(
        ISynQcoreDbContext context,
        ILogger<DeleteFeedPostHandler> logger,
        IMediator mediator,
        IFeedPostCacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _mediator = mediator;
        _cacheService = cacheService;
    }

    public async Task Handle(DeleteFeedPostCommand request, CancellationToken cancellationToken)
    {
        LogDeletingFeedPost(_logger, request.PostId, request.UserId);

        try
        {
            // Buscar o post
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);

            if (post == null)
            {
                LogPostNotFound(_logger, request.PostId);
                throw new InvalidOperationException("Post não encontrado");
            }

            // Verificar se o usuário pode excluir (apenas o autor)
            if (post.AuthorId != request.UserId)
            {
                LogUnauthorizedDelete(_logger, request.PostId, request.UserId, post.AuthorId);
                throw new UnauthorizedAccessException("Apenas o autor pode excluir este post");
            }

            // Marcar como excluído (soft delete)
            post.IsDeleted = true;
            post.DeletedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            LogFeedPostDeleted(_logger, post.Id, request.UserId);

            // Invalidar caches relacionados
            await _cacheService.RemovePostAsync(post.Id, cancellationToken);
            await _cacheService.InvalidateUserFeedAsync(request.UserId, cancellationToken);
            await _cacheService.InvalidateAllPostCachesAsync(cancellationToken);

            // Forçar regeneração do feed para remover o post excluído
            try
            {
                var regenerateCommand = new RegenerateFeedCommand
                {
                    UserId = request.UserId,
                    PreserveBookmarks = true,
                    DaysToInclude = 30
                };
                await _mediator.Send(regenerateCommand, cancellationToken);
            }
            catch (Exception ex)
            {
                LogFeedRegenerationError(_logger, ex, request.UserId);
                // Não falha a exclusão por erro na regeneração do feed
            }
        }
        catch (Exception ex)
        {
            LogErrorDeletingFeedPost(_logger, ex, request.PostId, request.UserId);
            throw;
        }
    }

    [LoggerMessage(EventId = 7201, Level = LogLevel.Information,
        Message = "Excluindo post no feed - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogDeletingFeedPost(ILogger logger, Guid postId, Guid userId);

    [LoggerMessage(EventId = 7202, Level = LogLevel.Warning,
        Message = "Post não encontrado para exclusão: {PostId}")]
    private static partial void LogPostNotFound(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 7203, Level = LogLevel.Warning,
        Message = "Tentativa não autorizada de excluir post - PostId: {PostId}, UserId: {UserId}, AuthorId: {AuthorId}")]
    private static partial void LogUnauthorizedDelete(ILogger logger, Guid postId, Guid userId, Guid authorId);

    [LoggerMessage(EventId = 7204, Level = LogLevel.Information,
        Message = "Post do feed excluído com sucesso - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogFeedPostDeleted(ILogger logger, Guid postId, Guid userId);

    [LoggerMessage(EventId = 7205, Level = LogLevel.Warning,
        Message = "Erro ao regenerar feed após exclusão - UserId: {UserId}")]
    private static partial void LogFeedRegenerationError(ILogger logger, Exception ex, Guid userId);

    [LoggerMessage(EventId = 7206, Level = LogLevel.Error,
        Message = "Erro ao excluir post no feed - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogErrorDeletingFeedPost(ILogger logger, Exception ex, Guid postId, Guid userId);
}

/// <summary>
/// Handler para obter um post individual do feed
/// </summary>
public partial class GetFeedPostHandler : IRequestHandler<GetFeedPostCommand, FeedPostDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetFeedPostHandler> _logger;
    private readonly IFeedPostCacheService _cacheService;

    public GetFeedPostHandler(
        ISynQcoreDbContext context,
        ILogger<GetFeedPostHandler> logger,
        IFeedPostCacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<FeedPostDto?> Handle(GetFeedPostCommand request, CancellationToken cancellationToken)
    {
        LogGettingFeedPost(_logger, request.PostId, request.UserId);

        try
        {
            // Tentar obter do cache primeiro
            var cachedPost = await _cacheService.GetPostAsync(request.PostId, cancellationToken);
            if (cachedPost != null)
            {
                // Incrementar view count no cache e banco
                await _cacheService.IncrementViewCountAsync(request.PostId, cancellationToken);
                
                // Incrementar no banco também
                var postForView = await _context.Posts
                    .FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);
                    
                if (postForView != null)
                {
                    postForView.ViewCount++;
                    await _context.SaveChangesAsync(cancellationToken);
                }

                LogFeedPostRetrieved(_logger, cachedPost.Id, request.UserId);
                return cachedPost;
            }

            // Buscar o post do banco com relacionamentos
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Author.EmployeeDepartments).ThenInclude(ed => ed.Department)
                .FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);

            if (post == null)
            {
                LogPostNotFound(_logger, request.PostId);
                return null;
            }

            LogFeedPostRetrieved(_logger, post.Id, request.UserId);

            // Incrementar view count
            post.ViewCount++;
            await _context.SaveChangesAsync(cancellationToken);

            // Criar DTO
            var postDto = new FeedPostDto
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                Tags = post.PostTags.Select(pt => pt.Tag.Name).ToList(),
                IsPublic = post.Visibility == PostVisibility.Company,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.FullName,
                AuthorAvatarUrl = post.Author.ProfilePhotoUrl,
                AuthorDepartment = post.Author.EmployeeDepartments.FirstOrDefault()?.Department?.Name,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                ViewCount = post.ViewCount,
                Status = post.Status.ToString(),
                Type = "FeedPost",
                Visibility = post.Visibility.ToString()
            };

            // Armazenar no cache
            await _cacheService.SetPostAsync(postDto, cancellationToken);

            return postDto;
        }
        catch (Exception ex)
        {
            LogErrorGettingFeedPost(_logger, ex, request.PostId, request.UserId);
            throw;
        }
    }

    [LoggerMessage(EventId = 7301, Level = LogLevel.Information,
        Message = "Obtendo post do feed - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogGettingFeedPost(ILogger logger, Guid postId, Guid userId);

    [LoggerMessage(EventId = 7302, Level = LogLevel.Information,
        Message = "Post não encontrado: {PostId}")]
    private static partial void LogPostNotFound(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 7303, Level = LogLevel.Information,
        Message = "Post do feed obtido com sucesso - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogFeedPostRetrieved(ILogger logger, Guid postId, Guid userId);

    [LoggerMessage(EventId = 7304, Level = LogLevel.Error,
        Message = "Erro ao obter post do feed - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogErrorGettingFeedPost(ILogger logger, Exception ex, Guid postId, Guid userId);
}