using MediatR;
using SynQcore.Application.DTOs.Admin;

namespace SynQcore.Application.Commands.Admin;

/// <summary>
/// Command para criação de novo usuário pelo administrador.
/// Processa dados de registro administrativo através do padrão CQRS.
/// </summary>
/// <param name="UserName">Nome de usuário único no sistema.</param>
/// <param name="Email">Email corporativo válido e único.</param>
/// <param name="Password">Senha inicial do usuário.</param>
/// <param name="PhoneNumber">Número de telefone (opcional).</param>
/// <param name="Role">Papel/perfil do usuário no sistema.</param>
public record CreateUserCommand(
    string UserName,
    string Email,
    string Password,
    string? PhoneNumber,
    string Role
) : IRequest<CreateUserResponse>;
