namespace SynQcore.Application.DTOs.Admin;

/// <summary>
/// Resposta da operação de criação de usuário pelo administrador.
/// Contém informações sobre o sucesso da operação e dados do usuário criado.
/// </summary>
public class CreateUserResponse
{
    /// <summary>
    /// Indica se a criação do usuário foi bem-sucedida.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem descritiva sobre o resultado da operação.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// ID do usuário criado (nulo em caso de falha).
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Nome do usuário criado.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Email do usuário criado.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Papel/perfil atribuído ao usuário criado.
    /// </summary>
    public string? Role { get; set; }
}
