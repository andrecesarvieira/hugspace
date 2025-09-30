/*
 * SynQcore - Corporate Social Network
 *
 * Moderation Queries - CQRS Queries para moderação
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Moderation.DTOs;

namespace SynQcore.Application.Features.Moderation.Queries;

/// <summary>
/// Query para obter fila de moderação com paginação e filtros
/// </summary>
public record GetModerationQueueQuery : IRequest<PagedResult<ModerationDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? Category { get; init; }
    public string? Severity { get; init; }
    public string? ContentType { get; init; }
    public bool? IsAutomaticDetection { get; init; }
    public Guid? ModeratorId { get; init; }
}

/// <summary>
/// Query para obter detalhes específicos de moderação
/// </summary>
public record GetModerationByIdQuery : IRequest<ModerationDto?>
{
    public Guid Id { get; init; }
}

/// <summary>
/// Query para obter estatísticas de moderação
/// </summary>
public record GetModerationStatsQuery : IRequest<ModerationStatsDto>;

/// <summary>
/// Query para obter categorias de moderação disponíveis
/// </summary>
public record GetModerationCategoriesQuery : IRequest<List<string>>;

/// <summary>
/// Query para obter severidades de moderação disponíveis
/// </summary>
public record GetModerationSeveritiesQuery : IRequest<List<string>>;

/// <summary>
/// Query para obter ações de moderação disponíveis
/// </summary>
public record GetModerationActionsQuery : IRequest<List<string>>;
