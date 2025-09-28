using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.DTOs.Auth;

/// <summary>
/// Request para registro de novo usuário no sistema.
/// Contém todos os dados necessários para criação de conta.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Nome de usuário único no sistema (obrigatório).
    /// </summary>
    [Required]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email corporativo do usuário (obrigatório e único).
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário (mínimo 8 caracteres, obrigatória).
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação da senha (deve ser idêntica à senha).
    /// </summary>
    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Número de telefone do usuário (opcional).
    /// </summary>
    public string? PhoneNumber { get; set; }
}
