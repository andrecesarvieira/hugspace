using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SynQcore.Infrastructure.Services.Auth;

/// <summary>
/// Serviço responsável por inicializar as roles corporativas no sistema
/// Cria as roles Employee, Manager, HR e Admin se elas não existirem
/// </summary>
public partial class RoleInitializationService
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<RoleInitializationService> _logger;

    public RoleInitializationService(
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<RoleInitializationService> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Inicializa todas as roles corporativas necessárias para o sistema
    /// Deve ser chamado durante a inicialização da aplicação
    /// </summary>
    public async Task InitializeRolesAsync()
    {
        var roles = new[] { "Employee", "Manager", "HR", "Admin" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<Guid> { Name = roleName };
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    LogRoleCreatedSuccessfully(_logger, roleName, null);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    LogRoleCreationError(_logger, roleName, errors, null);
                }
            }
            else
            {
                LogRoleAlreadyExists(_logger, roleName, null);
            }
        }
    }

    /// <summary>
    /// Método estático para facilitar o uso durante a inicialização da aplicação
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleInitService = scope.ServiceProvider.GetRequiredService<RoleInitializationService>();
        await roleInitService.InitializeRolesAsync();
    }

    // Delegates de log para melhor performance
    [LoggerMessage(LogLevel.Information, "Role corporativa '{roleName}' criada com sucesso")]
    private static partial void LogRoleCreatedSuccessfully(ILogger logger, string roleName, Exception? exception);

    [LoggerMessage(LogLevel.Error, "Erro ao criar role '{roleName}': {errors}")]
    private static partial void LogRoleCreationError(ILogger logger, string roleName, string errors, Exception? exception);

    [LoggerMessage(LogLevel.Debug, "Role corporativa '{roleName}' já existe no sistema")]
    private static partial void LogRoleAlreadyExists(ILogger logger, string roleName, Exception? exception);
}