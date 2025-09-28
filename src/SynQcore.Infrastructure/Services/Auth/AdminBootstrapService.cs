using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SynQcore.Infrastructure.Identity;

namespace SynQcore.Infrastructure.Services.Auth;

public partial class AdminBootstrapService
{
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminBootstrapService> _logger;

    public AdminBootstrapService(
        UserManager<ApplicationUserEntity> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration,
        ILogger<AdminBootstrapService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task CreateDefaultAdminIfNeededAsync()
    {
        // Verificar se já existe algum usuário com role Admin
        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
        if (adminUsers.Any())
        {
            LogAdminAlreadyExists(_logger, adminUsers.Count, null);
            return;
        }

        LogCreatingDefaultAdmin(_logger, null);

        // Obter configurações do administrador padrão
        var adminEmail = _configuration["DefaultAdmin:Email"] ?? "admin@synqcore.com";
        var adminPassword = _configuration["DefaultAdmin:Password"] ?? "SynQcore@Admin123!";
        var adminUserName = _configuration["DefaultAdmin:UserName"] ?? "admin";

        // Verificar se o usuário já existe (caso tenha sido criado sem role)
        var existingUser = await _userManager.FindByEmailAsync(adminEmail);
        
        if (existingUser == null)
        {
            // Criar novo usuário administrador
            var adminUser = new ApplicationUserEntity
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true // Admin padrão já vem confirmado
            };

            var createResult = await _userManager.CreateAsync(adminUser, adminPassword);
            
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                LogAdminCreationError(_logger, errors, null);
                return;
            }

            existingUser = adminUser;
            LogAdminUserCreated(_logger, adminEmail, null);
        }
        else
        {
            LogExistingUserPromoted(_logger, adminEmail, null);
        }

        // Garantir que a role Admin existe
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            var adminRole = new IdentityRole<Guid> { Name = "Admin" };
            await _roleManager.CreateAsync(adminRole);
            LogAdminRoleCreated(_logger, null);
        }

        // Atribuir role Admin ao usuário
        if (!await _userManager.IsInRoleAsync(existingUser, "Admin"))
        {
            var roleResult = await _userManager.AddToRoleAsync(existingUser, "Admin");
            
            if (roleResult.Succeeded)
            {
                LogAdminBootstrapCompleted(_logger, adminEmail, null);
            }
            else
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                LogRoleAssignmentError(_logger, errors, null);
            }
        }
        else
        {
            LogAdminAlreadyHasRole(_logger, adminEmail, null);
        }
    }

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var adminBootstrap = scope.ServiceProvider.GetRequiredService<AdminBootstrapService>();
        await adminBootstrap.CreateDefaultAdminIfNeededAsync();
    }

    // Delegates de log para melhor performance
    [LoggerMessage(LogLevel.Information, "Administrador padrão já existe. Total de admins: {adminCount}")]
    private static partial void LogAdminAlreadyExists(ILogger logger, int adminCount, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Nenhum administrador encontrado. Criando administrador padrão...")]
    private static partial void LogCreatingDefaultAdmin(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Usuário administrador criado: {email}")]
    private static partial void LogAdminUserCreated(ILogger logger, string email, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Usuário existente promovido a administrador: {email}")]
    private static partial void LogExistingUserPromoted(ILogger logger, string email, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Role Admin criada no sistema")]
    private static partial void LogAdminRoleCreated(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Bootstrap do administrador concluído com sucesso: {email}")]
    private static partial void LogAdminBootstrapCompleted(ILogger logger, string email, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Usuário {email} já possui role de administrador")]
    private static partial void LogAdminAlreadyHasRole(ILogger logger, string email, Exception? exception);

    [LoggerMessage(LogLevel.Error, "Erro ao criar usuário administrador: {errors}")]
    private static partial void LogAdminCreationError(ILogger logger, string errors, Exception? exception);

    [LoggerMessage(LogLevel.Error, "Erro ao atribuir role Admin: {errors}")]
    private static partial void LogRoleAssignmentError(ILogger logger, string errors, Exception? exception);
}