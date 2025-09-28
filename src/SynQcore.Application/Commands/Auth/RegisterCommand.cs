using MediatR;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Application.Commands.Auth;

/// <summary>
/// Command para registro de novo usuário no sistema.
/// Processa dados de auto-registro através do padrão CQRS.
/// </summary>
/// <param name="UserName">Nome de usuário único escolhido.</param>
/// <param name="Email">Email corporativo para registro.</param>
/// <param name="Password">Senha escolhida pelo usuário.</param>
/// <param name="ConfirmPassword">Confirmação da senha (deve ser idêntica).</param>
/// <param name="PhoneNumber">Número de telefone opcional.</param>
public record RegisterCommand(
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber
) : IRequest<AuthResponse>;
