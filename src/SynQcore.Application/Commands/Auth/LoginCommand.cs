using MediatR;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Application.Commands.Auth;

/// <summary>
/// Command para autenticação de usuário no sistema.
/// Processa credenciais de login através do padrão CQRS.
/// </summary>
/// <param name="Email">Email corporativo do usuário.</param>
/// <param name="Password">Senha do usuário.</param>
public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponse>;
