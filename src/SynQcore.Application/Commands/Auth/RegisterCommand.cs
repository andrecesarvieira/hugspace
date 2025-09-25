using MediatR;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Application.Commands.Auth;

public record RegisterCommand(
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber
) : IRequest<AuthResponse>;