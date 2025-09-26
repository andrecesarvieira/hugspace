using MediatR;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Application.Queries.Admin;

// Query para listar todos os usuários do sistema com paginação e filtro de busca
public record GetAllUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null
) : IRequest<UsersListResponse>;