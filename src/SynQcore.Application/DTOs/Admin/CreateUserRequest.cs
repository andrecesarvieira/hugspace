using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.DTOs.Admin;

/// <summary>
/// Request para criação de novo usuário pelo administrador.
/// Contém validações e dados necessários para o cadastro.
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Nome de usuário único (3-50 caracteres, obrigatório).
    /// </summary>
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 50 caracteres")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email corporativo válido e único (obrigatório).
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha inicial do usuário (mínimo 8 caracteres, obrigatória).
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(8, ErrorMessage = "Senha deve ter no mínimo 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Número de telefone com formato válido (opcional).
    /// </summary>
    [Phone(ErrorMessage = "Número de telefone deve ter formato válido")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Papel/perfil do usuário no sistema (obrigatório).
    /// </summary>
    [Required(ErrorMessage = "Papel é obrigatório")]
    public string Role { get; set; } = string.Empty;
}
