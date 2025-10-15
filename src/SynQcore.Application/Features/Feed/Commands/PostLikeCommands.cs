using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Feed.DTOs;

namespace SynQcore.Application.Features.Feed.Commands;

/// <summary>
/// Command para curtir um post no feed
/// </summary>
public record LikePostCommand : IRequest<PostLikeResponseDto>
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
    public string ReactionType { get; init; } = "Like"; // Like, Helpful, Insightful, Celebrate
}

/// <summary>
/// Command para remover curtida de um post no feed
/// </summary>
public record UnlikePostCommand : IRequest<PostLikeResponseDto>
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
}

/// <summary>
/// Query para obter status de curtida de um post
/// </summary>
public record GetPostLikeStatusQuery : IRequest<PostLikeStatusDto>
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
}

/// <summary>
/// Query para obter curtidas de um post
/// </summary>
public record GetPostLikesQuery : IRequest<PagedResult<PostLikeDto>>
{
    public Guid PostId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? ReactionType { get; init; }
}
