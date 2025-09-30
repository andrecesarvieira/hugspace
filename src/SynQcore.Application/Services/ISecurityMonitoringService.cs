/*
 * SynQcore - Corporate Social Network
 *
 * Security Monitoring Service Interface - Real-time security threat detection
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

namespace SynQcore.Application.Services;

/// <summary>
/// Interface para serviço de monitoramento de segurança em tempo real
/// </summary>
public interface ISecurityMonitoringService
{
    /// <summary>
    /// Monitora tentativas de login suspeitas
    /// </summary>
    Task MonitorLoginAttemptsAsync(string email, string ipAddress, bool success, CancellationToken cancellationToken = default);

    /// <summary>
    /// Monitora violações de rate limiting
    /// </summary>
    Task MonitorRateLimitViolationsAsync(string ipAddress, string endpoint, string? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Monitora tentativas de ataques de input (XSS, SQL injection)
    /// </summary>
    Task MonitorInputAttacksAsync(string inputType, string threatType, string ipAddress, string? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um IP deve ser bloqueado
    /// </summary>
    Task<bool> ShouldBlockIpAsync(string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona IP à lista de bloqueios temporários
    /// </summary>
    Task BlockIpTemporarilyAsync(string ipAddress, TimeSpan duration, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove IP da lista de bloqueios
    /// </summary>
    Task UnblockIpAsync(string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém métricas de segurança em tempo real
    /// </summary>
    Task<SecurityMetrics> GetSecurityMetricsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém alertas de segurança ativos
    /// </summary>
    Task<List<SecurityAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Detecta padrões de atividade suspeita
    /// </summary>
    Task DetectSuspiciousPatterns(CancellationToken cancellationToken = default);
}

/// <summary>
/// Métricas de segurança do sistema
/// </summary>
public class SecurityMetrics
{
    public int FailedLoginsLast24Hours { get; set; }
    public int RateLimitViolationsLast24Hours { get; set; }
    public int InputAttacksDetectedLast24Hours { get; set; }
    public int BlockedIpsCount { get; set; }
    public int ActiveSecurityAlertsCount { get; set; }
    public List<string> TopAttackingIps { get; set; } = new();
    public List<string> MostTargetedEndpoints { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Alerta de segurança
/// </summary>
public class SecurityAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string? IpAddress { get; set; }
    public string? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
