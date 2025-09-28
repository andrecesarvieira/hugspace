using Microsoft.AspNetCore.Identity;

namespace SynQcore.Application.DTOs.Auth;

/// <summary>
/// Resposta de autenticação após login ou registro.
/// Contém token JWT e informações do usuário autenticado.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Indica se a autenticação foi bem-sucedida.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem descritiva sobre o resultado da autenticação.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Token JWT para autenticação em requests subsequentes.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token de refresh para renovação do JWT.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora de expiração do token JWT.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Informações básicas do usuário autenticado.
    /// </summary>
    public UserInfo User { get; set; } = new();
}

/// <summary>
/// Informações básicas do usuário para resposta de autenticação.
/// Contém dados essenciais para identificação do usuário.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome de usuário para exibição.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email corporativo do usuário.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
