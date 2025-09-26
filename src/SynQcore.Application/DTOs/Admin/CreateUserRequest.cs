using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.DTOs.Admin;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 50 caracteres")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(8, ErrorMessage = "Senha deve ter no mínimo 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Número de telefone deve ter formato válido")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Papel é obrigatório")]
    public string Role { get; set; } = string.Empty;
}