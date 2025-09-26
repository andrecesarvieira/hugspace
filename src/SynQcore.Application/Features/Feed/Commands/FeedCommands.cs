using MediatR;

namespace SynQcore.Application.Features.Feed.Commands;

/// <summary>
/// Command para regenerar o feed de um usuário
/// Executa algoritmo de personalização e relevância
/// </summary>
public record RegenerateFeedCommand : IRequest
{
    public Guid UserId { get; init; }
    public bool PreserveBookmarks { get; init; } = true;
    public int DaysToInclude { get; init; } = 30;
    public int? MaxItems { get; init; }
}

/// <summary>
/// Command para marcar item do feed como lido
/// </summary>
public record MarkFeedItemAsReadCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid FeedEntryId { get; init; }
}

/// <summary>
/// Command para bookmark/unbookmark item do feed
/// </summary>
public record ToggleFeedBookmarkCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid FeedEntryId { get; init; }
}

/// <summary>
/// Command para ocultar item do feed
/// </summary>
public record HideFeedItemCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid FeedEntryId { get; init; }
    public string? Reason { get; init; }
}

/// <summary>
/// Command para atualizar interesses do usuário baseado em interação
/// </summary>
public record UpdateUserInterestsCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid ContentId { get; init; }
    public string InteractionType { get; init; } = string.Empty; // "view", "like", "comment", "share", "bookmark"
    public bool RecalculateScores { get; init; }
}

/// <summary>
/// Command para processamento em lote de feed para múltiplos usuários
/// Usado para posts novos ou atualizações em massa
/// </summary>
public record ProcessBulkFeedUpdateCommand : IRequest
{
    public List<Guid> PostIds { get; init; } = [];
    public string UpdateType { get; init; } = "new_post"; // "new_post", "post_updated", "post_deleted"
}