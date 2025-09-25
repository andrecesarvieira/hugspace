using MediatR;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Application.Commands.Auth;

public record LoginCommand(
    string Email,
    string Password
    
    ):IRequest<AuthResponse>;
