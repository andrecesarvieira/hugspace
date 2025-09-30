using SynQcore.BlazorApp.Models.Moderation;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Interface para serviços de moderação corporativa
/// </summary>
public interface IModerationService
{
    // Filas de moderação
    Task<ModerationQueueDto?> GetModerationQueueAsync(string? contentType = null, int page = 1, int pageSize = 20);
    Task<ModerationItemDto?> GetModerationItemAsync(int itemId);

    // Ações de moderação
    Task<bool> ApproveContentAsync(int itemId, string? reason = null);
    Task<bool> RejectContentAsync(int itemId, string reason);
    Task<bool> BulkApproveAsync(int[] itemIds);
    Task<bool> BulkRejectAsync(int[] itemIds, string reason);

    // Estatísticas
    Task<ModerationStatsDto?> GetModerationStatsAsync();
    Task<ModerationAnalyticsDto?> GetModerationAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);

    // Configurações
    Task<ModerationConfigDto?> GetModerationConfigAsync();
    Task<bool> UpdateModerationConfigAsync(ModerationConfigDto config);

    // Relatórios
    Task<ModerationReportDto?> GetModerationReportAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Implementação do serviço de moderação
/// </summary>
public class ModerationService : IModerationService
{
    private readonly IApiService _apiService;
    private const string BaseEndpoint = "api/moderation";

    public ModerationService(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// Obtém a fila de moderação com filtros opcionais
    /// </summary>
    public async Task<ModerationQueueDto?> GetModerationQueueAsync(string? contentType = null, int page = 1, int pageSize = 20)
    {
        var queryParams = new List<string>
        {
            $"page={page}",
            $"pageSize={pageSize}"
        };

        if (!string.IsNullOrEmpty(contentType))
        {
            queryParams.Add($"contentType={contentType}");
        }

        var endpoint = $"{BaseEndpoint}/queue?{string.Join("&", queryParams)}";
        return await _apiService.GetAsync<ModerationQueueDto>(endpoint);
    }

    /// <summary>
    /// Obtém detalhes de um item específico da moderação
    /// </summary>
    public async Task<ModerationItemDto?> GetModerationItemAsync(int itemId)
    {
        var endpoint = $"{BaseEndpoint}/items/{itemId}";
        return await _apiService.GetAsync<ModerationItemDto>(endpoint);
    }

    /// <summary>
    /// Aprova um conteúdo
    /// </summary>
    public async Task<bool> ApproveContentAsync(int itemId, string? reason = null)
    {
        var endpoint = $"{BaseEndpoint}/approve/{itemId}";
        var data = new { reason };
        return await _apiService.PostAsync(endpoint, data);
    }

    /// <summary>
    /// Rejeita um conteúdo
    /// </summary>
    public async Task<bool> RejectContentAsync(int itemId, string reason)
    {
        var endpoint = $"{BaseEndpoint}/reject/{itemId}";
        var data = new { reason };
        return await _apiService.PostAsync(endpoint, data);
    }

    /// <summary>
    /// Aprova múltiplos itens em lote
    /// </summary>
    public async Task<bool> BulkApproveAsync(int[] itemIds)
    {
        var endpoint = $"{BaseEndpoint}/bulk-approve";
        var data = new { itemIds };
        return await _apiService.PostAsync(endpoint, data);
    }

    /// <summary>
    /// Rejeita múltiplos itens em lote
    /// </summary>
    public async Task<bool> BulkRejectAsync(int[] itemIds, string reason)
    {
        var endpoint = $"{BaseEndpoint}/bulk-reject";
        var data = new { itemIds, reason };
        return await _apiService.PostAsync(endpoint, data);
    }

    /// <summary>
    /// Obtém estatísticas gerais de moderação
    /// </summary>
    public async Task<ModerationStatsDto?> GetModerationStatsAsync()
    {
        var endpoint = $"{BaseEndpoint}/stats";
        return await _apiService.GetAsync<ModerationStatsDto>(endpoint);
    }

    /// <summary>
    /// Obtém analytics detalhados de moderação
    /// </summary>
    public async Task<ModerationAnalyticsDto?> GetModerationAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = new List<string>();

        if (startDate.HasValue)
        {
            queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        }

        if (endDate.HasValue)
        {
            queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
        }

        var endpoint = $"{BaseEndpoint}/analytics";
        if (queryParams.Count > 0)
        {
            endpoint += $"?{string.Join("&", queryParams)}";
        }

        return await _apiService.GetAsync<ModerationAnalyticsDto>(endpoint);
    }

    /// <summary>
    /// Obtém configurações de moderação
    /// </summary>
    public async Task<ModerationConfigDto?> GetModerationConfigAsync()
    {
        var endpoint = $"{BaseEndpoint}/config";
        return await _apiService.GetAsync<ModerationConfigDto>(endpoint);
    }

    /// <summary>
    /// Atualiza configurações de moderação
    /// </summary>
    public async Task<bool> UpdateModerationConfigAsync(ModerationConfigDto config)
    {
        var endpoint = $"{BaseEndpoint}/config";
        return await _apiService.PostAsync(endpoint, config);
    }

    /// <summary>
    /// Gera relatório de moderação para período específico
    /// </summary>
    public async Task<ModerationReportDto?> GetModerationReportAsync(DateTime startDate, DateTime endDate)
    {
        var endpoint = $"{BaseEndpoint}/reports?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        return await _apiService.GetAsync<ModerationReportDto>(endpoint);
    }
}
