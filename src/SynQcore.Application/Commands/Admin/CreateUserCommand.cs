using MediatR;
using SynQcore.Application.DTOs.Admin;

namespace SynQcore.Application.Commands.Admin;

public record CreateUserCommand(
    string UserName,
    string Email,
    string Password,
    string? PhoneNumber,
    string Role
) : IRequest<CreateUserResponse>;