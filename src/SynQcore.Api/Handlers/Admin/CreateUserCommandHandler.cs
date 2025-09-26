using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Commands.Admin;
using SynQcore.Application.DTOs.Admin;
using SynQcore.Infrastructure.Identity;

namespace SynQcore.Api.Handlers.Admin;

public partial class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        UserManager<ApplicationUserEntity> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        LogCreatingUser(_logger, request.Email, request.Role);

        try
        {
            // Verificar se o email já existe
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                LogUserAlreadyExists(_logger, request.Email);
                return new CreateUserResponse 
                { 
                    Success = false, 
                    Message = $"Usuário com email '{request.Email}' já existe no sistema" 
                };
            }

            // Verificar se o role existe
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);
            if (!roleExists)
            {
                LogRoleNotFound(_logger, request.Role);
                return new CreateUserResponse 
                { 
                    Success = false, 
                    Message = $"Papel '{request.Role}' não existe no sistema" 
                };
            }

            // Criar o usuário
            var user = new ApplicationUserEntity
            {
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = true, // Usuários criados por admin são pré-confirmados
                PhoneNumber = request.PhoneNumber
            };

            // Criar usuário no banco
            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                LogUserCreationFailed(_logger, request.Email, errors);
                return new CreateUserResponse 
                { 
                    Success = false, 
                    Message = $"Falha ao criar usuário: {errors}" 
                };
            }

            // Atribuir o papel ao usuário
            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                LogRoleAssignmentFailed(_logger, request.Email, request.Role, roleErrors);
                
                // Se falhou ao atribuir o papel, remover o usuário criado
                await _userManager.DeleteAsync(user);
                
                return new CreateUserResponse 
                { 
                    Success = false, 
                    Message = $"Falha ao atribuir papel '{request.Role}': {roleErrors}" 
                };
            }

            LogUserCreatedSuccessfully(_logger, user.Id, request.Email, request.Role);

            return new CreateUserResponse
            {
                Success = true,
                Message = "Usuário criado com sucesso",
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = request.Role
            };
        }
        catch (Exception ex)
        {
            LogUnexpectedError(_logger, request.Email, ex.Message);
            return new CreateUserResponse 
            { 
                Success = false, 
                Message = "Erro interno do servidor ao criar usuário" 
            };
        }
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Iniciando criação de usuário administrativo: {Email} com papel {Role}")]
    private static partial void LogCreatingUser(ILogger logger, string email, string role);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Warning,
        Message = "Tentativa de criar usuário com email duplicado: {Email}")]
    private static partial void LogUserAlreadyExists(ILogger logger, string email);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Warning,
        Message = "Tentativa de atribuir papel inexistente: {Role}")]
    private static partial void LogRoleNotFound(ILogger logger, string role);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Error,
        Message = "Falha ao criar usuário {Email}: {Errors}")]
    private static partial void LogUserCreationFailed(ILogger logger, string email, string errors);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Error,
        Message = "Falha ao atribuir papel {Role} ao usuário {Email}: {Errors}")]
    private static partial void LogRoleAssignmentFailed(ILogger logger, string email, string role, string errors);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Information,
        Message = "Usuário criado com sucesso - ID: {UserId}, Email: {Email}, Papel: {Role}")]
    private static partial void LogUserCreatedSuccessfully(ILogger logger, Guid userId, string email, string role);

    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Error,
        Message = "Erro inesperado ao criar usuário {Email}: {ErrorMessage}")]
    private static partial void LogUnexpectedError(ILogger logger, string email, string errorMessage);
}