using MediatR;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Application.Queries.Users;

public record GetUserQuery(
    Guid UserId
) : IRequest<UserInfoDto?>;
