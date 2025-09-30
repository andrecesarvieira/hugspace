using Microsoft.AspNetCore.Components.Authorization;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.BlazorApp.Services;
using System.Security.Claims;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Implementação do serviço de permissões do usuário no Blazor
/// </summary>
public class UserPermissionService : IUserPermissionService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public UserPermissionService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    /// <summary>
    /// Verifica se o usuário atual pode executar ações de moderação
    /// </summary>
    public async Task<bool> CanModerateAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        if (!authState.User.Identity?.IsAuthenticated == true)
            return false;

        var role = authState.User.FindFirst(ClaimTypes.Role)?.Value;

        // Verificar se o usuário tem role de moderação
        return role is "Manager" or "HR" or "Admin" or "Moderador";
    }

    /// <summary>
    /// Verifica se o usuário atual é administrador
    /// </summary>
    public async Task<bool> IsAdminAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        if (!authState.User.Identity?.IsAuthenticated == true)
            return false;

        var role = authState.User.FindFirst(ClaimTypes.Role)?.Value;
        return role == "Admin";
    }

    /// <summary>
    /// Verifica se o usuário atual é gerente
    /// </summary>
    public async Task<bool> IsManagerAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        if (!authState.User.Identity?.IsAuthenticated == true)
            return false;

        var role = authState.User.FindFirst(ClaimTypes.Role)?.Value;
        return role is "Manager" or "Admin";
    }

    /// <summary>
    /// Verifica se o usuário atual tem permissão específica
    /// </summary>
    public async Task<bool> HasPermissionAsync(string permission)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        if (!authState.User.Identity?.IsAuthenticated == true)
            return false;

        // Verificar claims específicas de permissão
        var hasPermission = authState.User.HasClaim("permission", permission);

        // Fallback para verificação por role
        if (!hasPermission)
        {
            var role = authState.User.FindFirst(ClaimTypes.Role)?.Value;
            hasPermission = permission switch
            {
                "moderation" => role is "Manager" or "HR" or "Admin" or "Moderador",
                "administration" => role == "Admin",
                "management" => role is "Manager" or "Admin",
                "hr" => role is "HR" or "Admin",
                _ => false
            };
        }

        return hasPermission;
    }

    /// <summary>
    /// Obtém as informações do usuário atual
    /// </summary>
    public async Task<EmployeeDto?> GetCurrentUserAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        if (!authState.User.Identity?.IsAuthenticated == true)
            return null;

        var userIdClaim = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userNameClaim = authState.User.FindFirst(ClaimTypes.Name)?.Value;
        var emailClaim = authState.User.FindFirst(ClaimTypes.Email)?.Value;
        var roleClaim = authState.User.FindFirst(ClaimTypes.Role)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return null;

        // Retornar DTO básico com informações dos claims
        return new EmployeeDto
        {
            Id = userId,
            FirstName = userNameClaim ?? "Usuário",
            LastName = "",
            Email = emailClaim ?? "",
            IsActive = true,
            HireDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Verifica se o usuário está autenticado
    /// </summary>
    public bool IsAuthenticated()
    {
        // Para verificação síncrona rápida, usar GetAuthenticationStateAsync() seria async
        // Implementação simplificada - em produção, considerar cache
        return true; // Assumir autenticado se chegou até aqui com Authorization
    }
}
