/*
 * SynQcore - Corporate Social Network
 *
 * Advanced Rate Limiting Service Interface
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

namespace SynQcore.Application.Services;

/// <summary>
/// Interface para serviço avançado de rate limiting por IP com detecção de padrões suspeitos
/// </summary>
public interface IAdvancedRateLimitingService
{
    /// <summary>
    /// Verifica se uma requisição deve ser permitida baseado em rate limiting
    /// </summary>
    /// <param name="ipAddress">IP do cliente</param>
    /// <param name="endpoint">Endpoint sendo acessado</param>
    /// <param name="userId">ID do usuário (se autenticado)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da verificação de rate limit</returns>
    Task<RateLimitResult> CheckRateLimitAsync(string ipAddress, string endpoint, string? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra uma requisição para tracking e análise
    /// </summary>
    /// <param name="ipAddress">IP do cliente</param>
    /// <param name="endpoint">Endpoint acessado</param>
    /// <param name="successful">Se a requisição foi bem-sucedida</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task RecordRequestAsync(string ipAddress, string endpoint, bool successful, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém lista de IPs atualmente bloqueados
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de IPs bloqueados</returns>
    Task<List<string>> GetBlockedIpsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um IP da lista de bloqueados
    /// </summary>
    /// <param name="ipAddress">IP a ser desbloqueado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task UnblockIpAsync(string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém estatísticas de rate limiting
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Estatísticas detalhadas</returns>
    Task<RateLimitStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Resultado de verificação de rate limit
/// </summary>
public class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public int RemainingRequests { get; set; }
    public DateTime ResetTime { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// Estatísticas de rate limiting
/// </summary>
public class RateLimitStatistics
{
    public int UniqueIpsInWindow { get; set; }
    public int BlockedIpsCount { get; set; }
    public int TotalRequestsInWindow { get; set; }
    public Dictionary<string, int> TopIpsByVolume { get; set; } = new();
    public Dictionary<string, int> TopEndpointsByVolume { get; set; } = new();
    public Dictionary<string, int> TopViolatingIps { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public int TimeWindowMinutes { get; set; }
}

/// <summary>
/// Métrica de requisição para cache
/// </summary>
public class RequestMetric
{
    public string IpAddress { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public bool Successful { get; set; }
    public DateTime Timestamp { get; set; }
}
