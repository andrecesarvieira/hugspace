using MediatR;
using SynQcore.Application.DTOs;

namespace SynQcore.Application.Features.Feed.Queries;

public record GetUserInterestsQuery : IRequest<UserInterestsResponseDto>
{
    public Guid UserId { get; init; }
}
