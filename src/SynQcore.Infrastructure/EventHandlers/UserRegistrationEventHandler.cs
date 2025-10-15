using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Services;
using SynQcore.Infrastructure.Identity;

namespace SynQcore.Infrastructure.EventHandlers;

/// <summary>
/// Service para manipular eventos de registro de usuário
/// </summary>
public partial class UserRegistrationService
{
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly ILogger<UserRegistrationService> _logger;

    public UserRegistrationService(
        IEmployeeSyncService employeeSyncService,
        ILogger<UserRegistrationService> logger)
    {
        _employeeSyncService = employeeSyncService;
        _logger = logger;
    }

    /// <summary>
    /// Manipula evento de usuário registrado com sucesso
    /// </summary>
    public async Task HandleUserRegisteredAsync(ApplicationUserEntity user)
    {
        try
        {
            LogHandlingUserRegistration(_logger, user.Id, user.Email ?? "email não informado");

            // Criar Employee automaticamente para o novo usuário
            var employee = await _employeeSyncService.CreateEmployeeFromIdentityUserAsync(user.Id);

            LogEmployeeCreatedForUser(_logger, user.Id, employee.Email);
        }
        catch (Exception ex)
        {
            LogErrorCreatingEmployee(_logger, user.Id, ex);
            // Não relançar a exceção para não afetar o processo de registro
        }
    }

    #region LoggerMessage delegates

    [LoggerMessage(EventId = 5001, Level = LogLevel.Information,
        Message = "Manipulando registro de usuário {UserId} ({Email})")]
    private static partial void LogHandlingUserRegistration(ILogger logger, Guid userId, string email);

    [LoggerMessage(EventId = 5002, Level = LogLevel.Information,
        Message = "Employee criado automaticamente para usuário {UserId} ({Email})")]
    private static partial void LogEmployeeCreatedForUser(ILogger logger, Guid userId, string email);

    [LoggerMessage(EventId = 5003, Level = LogLevel.Error,
        Message = "Erro ao criar Employee para usuário {UserId}")]
    private static partial void LogErrorCreatingEmployee(ILogger logger, Guid userId, Exception ex);

    #endregion
}
