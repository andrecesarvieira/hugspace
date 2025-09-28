using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.DTOs.Auth;

/// <summary>
/// Request para autenticação de usuário no sistema.
/// Contém as credenciais necessárias para login.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email corporativo do usuário (obrigatório).
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário (obrigatória).
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
