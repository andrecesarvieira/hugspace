using MediatR;
using SynQcore.Application.DTOs;

namespace SynQcore.Application.Features.Feed.Queries;

/// <summary>
/// Query para obter interesses do usuário
/// </summary>
public record GetUserInterestsQuery : IRequest<UserInterestsResponseDto>
{
    public Guid UserId { get; init; }
}