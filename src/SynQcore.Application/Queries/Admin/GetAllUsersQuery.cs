using MediatR;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Application.Queries.Admin;

/// <summary>
/// Query para listar todos os usuários do sistema com paginação e filtro de busca.
/// Utilizada por administradores para gerenciar usuários da plataforma.
/// </summary>
/// <param name="Page">Número da página para paginação (padrão: 1).</param>
/// <param name="PageSize">Tamanho da página (padrão: 10).</param>
/// <param name="SearchTerm">Termo de busca para filtrar por nome ou email.</param>
public record GetAllUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null
) : IRequest<UsersListResponse>;
