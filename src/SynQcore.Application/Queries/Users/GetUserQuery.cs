using MediatR;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Application.Queries.Users;

/// <summary>
/// Query para obter informações de um usuário específico.
/// Utilizada para consulta de dados detalhados de usuário.
/// </summary>
/// <param name="UserId">Identificador único do usuário a ser consultado.</param>
public record GetUserQuery(
    Guid UserId
) : IRequest<UserInfoDto?>;
