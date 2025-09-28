namespace SynQcore.Application.Common.Interfaces;

/// <summary>
/// Interface para serviço de usuário atual corporativo.
/// Fornece informações do contexto do usuário autenticado.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// ID do usuário atual autenticado.
    /// Lança exceção se não houver usuário autenticado.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Nome de usuário (email) do usuário atual.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Papel/perfil corporativo do usuário atual no sistema.
    /// </summary>
    string? Role { get; }

    /// <summary>
    /// ID do departamento do usuário atual.
    /// Nulo se o usuário não estiver associado a um departamento.
    /// </summary>
    Guid? DepartmentId { get; }

    /// <summary>
    /// Indica se o usuário atual tem permissões de moderação.
    /// </summary>
    bool CanModerate { get; }

    /// <summary>
    /// Indica se o usuário atual tem perfil de administrador.
    /// </summary>
    bool IsAdmin { get; }
}
