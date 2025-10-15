using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Feed.Commands;
using SynQcore.Application.Features.Feed.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Feed.Handlers;

/// <summary>
/// Handler para criação de posts no feed corporativo
/// </summary>
public partial class CreateFeedPostHandler : IRequestHandler<CreateFeedPostCommand, FeedPostDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<CreateFeedPostHandler> _logger;
    private readonly IMediator _mediator;

    public CreateFeedPostHandler(
        ISynQcoreDbContext context,
        ILogger<CreateFeedPostHandler> logger,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<FeedPostDto> Handle(CreateFeedPostCommand request, CancellationToken cancellationToken)
    {
        LogCreatingFeedPost(_logger, request.AuthorId, request.Content.Length);

        try
        {
            // Verificar se o autor existe
            var author = await _context.Employees
                .Include(e => e.EmployeeDepartments)
                    .ThenInclude(ed => ed.Department)
                .FirstOrDefaultAsync(e => e.Id == request.AuthorId && !e.IsDeleted, cancellationToken);

            if (author == null)
            {
                LogAuthorNotFound(_logger, request.AuthorId);
                throw new InvalidOperationException("Autor não encontrado ou inativo");
            }

            // Criar o post
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = $"Post de {author.FullName}",
                Content = request.Content,
                Summary = request.Content.Length > 100 ? request.Content[..100] + "..." : request.Content,
                ImageUrl = request.ImageUrl,
                Type = PostType.Post,
                Status = PostStatus.Published,
                Visibility = request.IsPublic ? PostVisibility.Company : PostVisibility.Department,
                RequiresApproval = false,
                AuthorId = request.AuthorId,
                DepartmentId = author.EmployeeDepartments.FirstOrDefault()?.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                LikeCount = 0,
                CommentCount = 0,
                ViewCount = 0
            };

            _context.Posts.Add(post);

            // Processar tags se fornecidas
            if (request.Tags?.Length > 0)
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

            await _context.SaveChangesAsync(cancellationToken);

            LogFeedPostCreated(_logger, post.Id, request.AuthorId);

            // Forçar regeneração do feed para que o novo post apareça imediatamente
            try
            {
                var regenerateCommand = new RegenerateFeedCommand
                {
                    UserId = request.AuthorId,
                    PreserveBookmarks = true,
                    DaysToInclude = 30
                };
                await _mediator.Send(regenerateCommand, cancellationToken);
                Console.WriteLine($"[CREATE POST] Feed regenerado automaticamente para usuário {request.AuthorId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CREATE POST] Erro ao regenerar feed: {ex.Message}");
                // Não falha a criação do post por erro na regeneração do feed
            }

            // Retornar DTO
            return new FeedPostDto
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                Tags = request.Tags?.ToList() ?? [],
                IsPublic = request.IsPublic,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId,
                AuthorName = author.FullName,
                AuthorAvatarUrl = author.ProfilePhotoUrl,
                AuthorDepartment = author.EmployeeDepartments.FirstOrDefault()?.Department?.Name,
                LikeCount = 0,
                CommentCount = 0,
                ViewCount = 0,
                Status = "Published",
                Type = "FeedPost",
                Visibility = request.IsPublic ? "Company" : "Department"
            };
        }
        catch (Exception ex)
        {
            LogErrorCreatingFeedPost(_logger, ex, request.AuthorId);
            throw;
        }
    }

    [LoggerMessage(EventId = 7001, Level = LogLevel.Information,
        Message = "Criando post no feed - Autor: {AuthorId}, Tamanho: {ContentLength}")]
    private static partial void LogCreatingFeedPost(ILogger logger, Guid authorId, int contentLength);

    [LoggerMessage(EventId = 7002, Level = LogLevel.Warning,
        Message = "Autor não encontrado: {AuthorId}")]
    private static partial void LogAuthorNotFound(ILogger logger, Guid authorId);

    [LoggerMessage(EventId = 7003, Level = LogLevel.Information,
        Message = "Post do feed criado com sucesso - PostId: {PostId}, Autor: {AuthorId}")]
    private static partial void LogFeedPostCreated(ILogger logger, Guid postId, Guid authorId);

    [LoggerMessage(EventId = 7004, Level = LogLevel.Error,
        Message = "Erro ao criar post no feed - Autor: {AuthorId}")]
    private static partial void LogErrorCreatingFeedPost(ILogger logger, Exception ex, Guid authorId);
}
