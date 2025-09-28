using MediatR;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Application.Queries.Admin;

public record GetAllUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null
) : IRequest<UsersListResponse>;
