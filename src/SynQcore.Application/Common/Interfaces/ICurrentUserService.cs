namespace SynQcore.Application.Common.Interfaces;

// Interface para serviço de usuário atual corporativo
public interface ICurrentUserService
{
    // ID do usuário atual autenticado (lança exceção se não autenticado)
    Guid UserId { get; }
    
    // Nome de usuário (email) do usuário atual
    string? UserName { get; }
    
    // Papel corporativo do usuário atual
    string? Role { get; }
    
    // Departamento ID do usuário atual (nullable pois pode não ter)
    Guid? DepartmentId { get; }
    
    // Verifica se o usuário atual tem permissão para moderar
    bool CanModerate { get; }
    
    // Verifica se o usuário atual é admin
    bool IsAdmin { get; }
}