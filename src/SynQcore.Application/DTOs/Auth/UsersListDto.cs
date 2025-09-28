namespace SynQcore.Application.DTOs.Auth;

/// <summary>
/// DTO contendo informações detalhadas de um usuário para visualização administrativa.
/// Inclui dados de segurança e status da conta.
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome de usuário no sistema.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email corporativo do usuário.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o email foi confirmado.
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Número de telefone do usuário (opcional).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Indica se o telefone foi confirmado.
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Indica se a autenticação de dois fatores está habilitada.
    /// </summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Indica se o bloqueio de conta está habilitado.
    /// </summary>
    public bool LockoutEnabled { get; set; }

    /// <summary>
    /// Data e hora de término do bloqueio (se houver).
    /// </summary>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Número de tentativas de acesso falhadas.
    /// </summary>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Lista de papéis/perfis atribuídos ao usuário.
    /// </summary>
    public List<string> Roles { get; set; } = new();
}

/// <summary>
/// DTO para resposta da listagem de usuários com informações de paginação.
/// Contém dados paginados e metadados da consulta.
/// </summary>
public class UsersListResponse
{
    /// <summary>
    /// Lista de usuários da página atual.
    /// </summary>
    public List<UserInfoDto> Users { get; set; } = new();

    /// <summary>
    /// Total de usuários encontrados na busca.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Número da página atual.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de páginas disponíveis.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indica se há página anterior.
    /// </summary>
    public bool HasPrevious { get; set; }

    /// <summary>
    /// Indica se há próxima página.
    /// </summary>
    public bool HasNext { get; set; }
}
