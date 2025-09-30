using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Interface para serviço de permissões do usuário no Blazor
/// </summary>
public interface IUserPermissionService
{
    /// <summary>
    /// Verifica se o usuário atual pode executar ações de moderação
    /// </summary>
    Task<bool> CanModerateAsync();

    /// <summary>
    /// Verifica se o usuário atual é administrador
    /// </summary>
    Task<bool> IsAdminAsync();

    /// <summary>
    /// Verifica se o usuário atual é gerente
    /// </summary>
    Task<bool> IsManagerAsync();

    /// <summary>
    /// Verifica se o usuário atual tem permissão específica
    /// </summary>
    Task<bool> HasPermissionAsync(string permission);

    /// <summary>
    /// Obtém as informações do usuário atual
    /// </summary>
    Task<EmployeeDto?> GetCurrentUserAsync();

    /// <summary>
    /// Verifica se o usuário está autenticado
    /// </summary>
    bool IsAuthenticated();
}
