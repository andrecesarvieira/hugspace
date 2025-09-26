using Microsoft.AspNetCore.Http;
using SynQcore.Application.Common.Interfaces;
using System.Security.Claims;

namespace SynQcore.Infrastructure.Services;

// Implementação do serviço de usuário atual corporativo
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => 
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) is { } userIdString && 
        Guid.TryParse(userIdString, out var userId) ? userId : 
        throw new UnauthorizedAccessException("Usuário não autenticado ou ID inválido");

    public string? UserName => 
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public string? Role => 
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public Guid? DepartmentId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("DepartmentId") is { } deptIdString && 
        Guid.TryParse(deptIdString, out var deptId) ? deptId : null;

    public bool CanModerate => 
        Role is "Manager" or "HR" or "Admin";

    public bool IsAdmin => 
        Role == "Admin";
}