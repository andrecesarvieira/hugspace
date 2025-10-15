/*
 * SynQcore - Corporate Social Network
 *
 * Implementação do Serviço de Gestão de Endorsements
 * Fornece integração completa com API de validação de conhecimento
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using System.Globalization;
using System.Text;
using System.Text.Json;
using SynQcore.BlazorApp.Models;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Implementação do serviço de endorsements corporativos
/// Integra com a API backend para gerenciamento de validações de conhecimento
/// </summary>
public partial class EndorsementService : IEndorsementService
{
    private readonly IApiService _apiService;
    private readonly ILogger<EndorsementService> _logger;

    // Event IDs para logging estruturado (4001-4032)
    [LoggerMessage(EventId = 4001, Level = LogLevel.Information,
        Message = "Buscando endorsements - Filtros: PostId={PostId}, EndorserId={EndorserId}, Type={Type}, Page={Page}, PageSize={PageSize}")]
    private static partial void LogSearchEndorsements(ILogger logger, Guid? postId, Guid? endorserId, EndorsementType? type, int page, int pageSize);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Information,
        Message = "Busca de endorsements concluída - Total encontrado: {TotalCount}, Página {Page} de {TotalPages}")]
    private static partial void LogSearchEndorsementsSuccess(ILogger logger, int totalCount, int page, int totalPages);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Information,
        Message = "Obtendo endorsement por ID: {EndorsementId}")]
    private static partial void LogGetEndorsementById(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 4004, Level = LogLevel.Information,
        Message = "Endorsement encontrado - ID: {EndorsementId}, Type: {Type}, Endorser: {EndorserName}")]
    private static partial void LogGetEndorsementByIdSuccess(ILogger logger, Guid endorsementId, EndorsementType type, string endorserName);

    [LoggerMessage(EventId = 4005, Level = LogLevel.Warning,
        Message = "Endorsement não encontrado - ID: {EndorsementId}")]
    private static partial void LogEndorsementNotFound(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 4006, Level = LogLevel.Information,
        Message = "Obtendo endorsements do post: {PostId}, FilterType: {FilterType}, IncludePrivate: {IncludePrivate}")]
    private static partial void LogGetPostEndorsements(ILogger logger, Guid postId, EndorsementType? filterType, bool includePrivate);

    [LoggerMessage(EventId = 4007, Level = LogLevel.Information,
        Message = "Endorsements do post obtidos - PostId: {PostId}, Total: {Count}")]
    private static partial void LogGetPostEndorsementsSuccess(ILogger logger, Guid postId, int count);

    [LoggerMessage(EventId = 4008, Level = LogLevel.Information,
        Message = "Obtendo estatísticas de endorsements - PostId: {PostId}, CommentId: {CommentId}")]
    private static partial void LogGetEndorsementStats(ILogger logger, Guid? postId, Guid? commentId);

    [LoggerMessage(EventId = 4009, Level = LogLevel.Information,
        Message = "Estatísticas obtidas - Total: {TotalEndorsements}, TopType: {TopType}")]
    private static partial void LogGetEndorsementStatsSuccess(ILogger logger, int totalEndorsements, EndorsementType? topType);

    [LoggerMessage(EventId = 4010, Level = LogLevel.Information,
        Message = "Obtendo analytics de endorsements - Período: {StartDate} a {EndDate}, Department: {DepartmentId}")]
    private static partial void LogGetEndorsementAnalytics(ILogger logger, DateTime? startDate, DateTime? endDate, Guid? departmentId);

    [LoggerMessage(EventId = 4011, Level = LogLevel.Information,
        Message = "Analytics obtido - Total: {TotalEndorsements}, Participantes: {TotalParticipants}, GrowthRate: {GrowthRate}%")]
    private static partial void LogGetEndorsementAnalyticsSuccess(ILogger logger, int totalEndorsements, int totalParticipants, double growthRate);

    [LoggerMessage(EventId = 4012, Level = LogLevel.Information,
        Message = "Criando endorsement - Type: {Type}, PostId: {PostId}, CommentId: {CommentId}, IsPublic: {IsPublic}")]
    private static partial void LogCreateEndorsement(ILogger logger, EndorsementType type, Guid? postId, Guid? commentId, bool isPublic);

    [LoggerMessage(EventId = 4013, Level = LogLevel.Information,
        Message = "Endorsement criado com sucesso - ID: {EndorsementId}, Type: {Type}")]
    private static partial void LogCreateEndorsementSuccess(ILogger logger, Guid endorsementId, EndorsementType type);

    [LoggerMessage(EventId = 4014, Level = LogLevel.Information,
        Message = "Atualizando endorsement - ID: {EndorsementId}, Type: {Type}, IsPublic: {IsPublic}")]
    private static partial void LogUpdateEndorsement(ILogger logger, Guid endorsementId, EndorsementType? type, bool? isPublic);

    [LoggerMessage(EventId = 4015, Level = LogLevel.Information,
        Message = "Endorsement atualizado com sucesso - ID: {EndorsementId}")]
    private static partial void LogUpdateEndorsementSuccess(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 4016, Level = LogLevel.Information,
        Message = "Excluindo endorsement - ID: {EndorsementId}, Reason: {Reason}")]
    private static partial void LogDeleteEndorsement(ILogger logger, Guid endorsementId, string? reason);

    [LoggerMessage(EventId = 4017, Level = LogLevel.Information,
        Message = "Endorsement excluído com sucesso - ID: {EndorsementId}")]
    private static partial void LogDeleteEndorsementSuccess(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 4018, Level = LogLevel.Information,
        Message = "Toggle endorsement - Type: {Type}, PostId: {PostId}, CommentId: {CommentId}")]
    private static partial void LogToggleEndorsement(ILogger logger, EndorsementType type, Guid? postId, Guid? commentId);

    [LoggerMessage(EventId = 4019, Level = LogLevel.Information,
        Message = "Toggle endorsement concluído - Resultado: {Action}")]
    private static partial void LogToggleEndorsementSuccess(ILogger logger, string action);

    [LoggerMessage(EventId = 4020, Level = LogLevel.Information,
        Message = "Verificando endorsement do usuário - PostId: {PostId}, CommentId: {CommentId}, UserId: {UserId}")]
    private static partial void LogCheckUserEndorsement(ILogger logger, Guid? postId, Guid? commentId, Guid? userId);

    [LoggerMessage(EventId = 4021, Level = LogLevel.Information,
        Message = "Verificação concluída - Usuário tem endorsement: {HasEndorsement}")]
    private static partial void LogCheckUserEndorsementSuccess(ILogger logger, bool hasEndorsement);

    [LoggerMessage(EventId = 4022, Level = LogLevel.Error,
        Message = "Erro ao buscar endorsements - Erro: {Error}")]
    private static partial void LogSearchEndorsementsError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(EventId = 4023, Level = LogLevel.Error,
        Message = "Erro ao obter endorsement por ID - EndorsementId: {EndorsementId}, Erro: {Error}")]
    private static partial void LogGetEndorsementByIdError(ILogger logger, Guid endorsementId, string error, Exception? exception);

    [LoggerMessage(EventId = 4024, Level = LogLevel.Error,
        Message = "Erro ao obter endorsements do post - PostId: {PostId}, Erro: {Error}")]
    private static partial void LogGetPostEndorsementsError(ILogger logger, Guid postId, string error, Exception? exception);

    [LoggerMessage(EventId = 4025, Level = LogLevel.Error,
        Message = "Erro ao obter estatísticas - Erro: {Error}")]
    private static partial void LogGetEndorsementStatsError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(EventId = 4026, Level = LogLevel.Error,
        Message = "Erro ao obter analytics - Erro: {Error}")]
    private static partial void LogGetEndorsementAnalyticsError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(EventId = 4027, Level = LogLevel.Error,
        Message = "Erro ao criar endorsement - Erro: {Error}")]
    private static partial void LogCreateEndorsementError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(EventId = 4028, Level = LogLevel.Error,
        Message = "Erro ao atualizar endorsement - ID: {EndorsementId}, Erro: {Error}")]
    private static partial void LogUpdateEndorsementError(ILogger logger, Guid endorsementId, string error, Exception? exception);

    [LoggerMessage(EventId = 4029, Level = LogLevel.Error,
        Message = "Erro ao excluir endorsement - ID: {EndorsementId}, Erro: {Error}")]
    private static partial void LogDeleteEndorsementError(ILogger logger, Guid endorsementId, string error, Exception? exception);

    [LoggerMessage(EventId = 4030, Level = LogLevel.Error,
        Message = "Erro ao fazer toggle de endorsement - Erro: {Error}")]
    private static partial void LogToggleEndorsementError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(EventId = 4031, Level = LogLevel.Error,
        Message = "Erro ao verificar endorsement do usuário - Erro: {Error}")]
    private static partial void LogCheckUserEndorsementError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(EventId = 4032, Level = LogLevel.Warning,
        Message = "Usando dados de fallback para endorsements - Motivo: {Reason}")]
    private static partial void LogUsingFallbackData(ILogger logger, string reason);

    public EndorsementService(IApiService apiService, ILogger<EndorsementService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResult<EndorsementDto>> SearchEndorsementsAsync(EndorsementSearchRequest searchRequest)
    {
        LogSearchEndorsements(_logger, searchRequest.PostId, searchRequest.EndorserId, searchRequest.Type, searchRequest.Page, searchRequest.PageSize);

        try
        {
            var response = await _apiService.PostAsync<PagedResult<EndorsementDto>>("api/collaboration/search", searchRequest);

            if (response != null)
            {
                LogSearchEndorsementsSuccess(_logger, response.TotalCount, response.Page, response.TotalPages);
                return response;
            }

            LogUsingFallbackData(_logger, "API retornou null");
            return GetFallbackEndorsements(searchRequest);
        }
        catch (Exception ex)
        {
            LogSearchEndorsementsError(_logger, ex.Message, ex);
            LogUsingFallbackData(_logger, $"Erro na API: {ex.Message}");
            return GetFallbackEndorsements(searchRequest);
        }
    }

    public async Task<EndorsementDto?> GetEndorsementByIdAsync(Guid id)
    {
        LogGetEndorsementById(_logger, id);

        try
        {
            var response = await _apiService.GetAsync<EndorsementDto>($"api/collaboration/{id}");

            if (response != null)
            {
                LogGetEndorsementByIdSuccess(_logger, response.Id, response.Type, response.EndorserName);
                return response;
            }

            LogEndorsementNotFound(_logger, id);
            return null;
        }
        catch (Exception ex)
        {
            LogGetEndorsementByIdError(_logger, id, ex.Message, ex);
            return null;
        }
    }

    public async Task<List<EndorsementDto>> GetPostEndorsementsAsync(Guid postId, EndorsementType? filterByType = null, bool includePrivate = false)
    {
        LogGetPostEndorsements(_logger, postId, filterByType, includePrivate);

        try
        {
            var queryParams = BuildQueryParams(new Dictionary<string, object?>
            {
                ["filterByType"] = filterByType?.ToString(),
                ["includePrivate"] = includePrivate
            });

            var response = await _apiService.GetAsync<List<EndorsementDto>>($"api/collaboration/post/{postId}{queryParams}");

            if (response != null)
            {
                LogGetPostEndorsementsSuccess(_logger, postId, response.Count);
                return response;
            }

            LogUsingFallbackData(_logger, "API retornou null");
            return GetFallbackPostEndorsements(postId);
        }
        catch (Exception ex)
        {
            LogGetPostEndorsementsError(_logger, postId, ex.Message, ex);
            LogUsingFallbackData(_logger, $"Erro na API: {ex.Message}");
            return GetFallbackPostEndorsements(postId);
        }
    }

    public async Task<EndorsementStatsDto> GetEndorsementStatsAsync(Guid? postId = null, Guid? commentId = null, bool includePrivate = false)
    {
        LogGetEndorsementStats(_logger, postId, commentId);

        try
        {
            var queryParams = BuildQueryParams(new Dictionary<string, object?>
            {
                ["postId"] = postId,
                ["commentId"] = commentId,
                ["includePrivate"] = includePrivate
            });

            var response = await _apiService.GetAsync<EndorsementStatsDto>($"api/collaboration/stats{queryParams}");

            if (response != null)
            {
                LogGetEndorsementStatsSuccess(_logger, response.TotalEndorsements, response.TopEndorsementType);
                return response;
            }

            LogUsingFallbackData(_logger, "API retornou null");
            return GetFallbackStats(postId, commentId);
        }
        catch (Exception ex)
        {
            LogGetEndorsementStatsError(_logger, ex.Message, ex);
            LogUsingFallbackData(_logger, $"Erro na API: {ex.Message}");
            return GetFallbackStats(postId, commentId);
        }
    }

    public async Task<EndorsementAnalyticsDto> GetEndorsementAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null, Guid? departmentId = null, bool includePrivate = false)
    {
        LogGetEndorsementAnalytics(_logger, startDate, endDate, departmentId);

        try
        {
            var queryParams = BuildQueryParams(new Dictionary<string, object?>
            {
                ["startDate"] = startDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ["endDate"] = endDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ["departmentId"] = departmentId,
                ["includePrivate"] = includePrivate
            });

            var response = await _apiService.GetAsync<EndorsementAnalyticsDto>($"api/collaboration/analytics{queryParams}");

            if (response != null)
            {
                LogGetEndorsementAnalyticsSuccess(_logger, response.TotalEndorsements, response.TotalParticipants, response.GrowthRate);
                return response;
            }

            LogUsingFallbackData(_logger, "API retornou null");
            return GetFallbackAnalytics();
        }
        catch (Exception ex)
        {
            LogGetEndorsementAnalyticsError(_logger, ex.Message, ex);
            LogUsingFallbackData(_logger, $"Erro na API: {ex.Message}");
            return GetFallbackAnalytics();
        }
    }

    public async Task<EndorsementDto> CreateEndorsementAsync(CreateEndorsementRequest request)
    {
        LogCreateEndorsement(_logger, request.Type, request.PostId, request.CommentId, request.IsPublic);

        try
        {
            var response = await _apiService.PostAsync<EndorsementDto>("api/collaboration", request);

            if (response != null)
            {
                LogCreateEndorsementSuccess(_logger, response.Id, response.Type);
                return response;
            }

            throw new InvalidOperationException("Erro ao criar endorsement: resposta da API foi null");
        }
        catch (Exception ex)
        {
            LogCreateEndorsementError(_logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<EndorsementDto> UpdateEndorsementAsync(Guid id, UpdateEndorsementRequest request)
    {
        LogUpdateEndorsement(_logger, id, request.Type, request.IsPublic);

        try
        {
            var response = await _apiService.PutAsync<EndorsementDto>($"api/collaboration/{id}", request);

            if (response != null)
            {
                LogUpdateEndorsementSuccess(_logger, response.Id);
                return response;
            }

            throw new InvalidOperationException("Erro ao atualizar endorsement: resposta da API foi null");
        }
        catch (Exception ex)
        {
            LogUpdateEndorsementError(_logger, id, ex.Message, ex);
            throw;
        }
    }

    public async Task<bool> DeleteEndorsementAsync(Guid id, string? reason = null)
    {
        LogDeleteEndorsement(_logger, id, reason);

        try
        {
            await _apiService.DeleteAsync($"api/collaboration/{id}");

            LogDeleteEndorsementSuccess(_logger, id);
            return true;
        }
        catch (Exception ex)
        {
            LogDeleteEndorsementError(_logger, id, ex.Message, ex);
            return false;
        }
    }

    public async Task<EndorsementDto?> ToggleEndorsementAsync(ToggleEndorsementRequest request)
    {
        LogToggleEndorsement(_logger, request.Type, request.PostId, request.CommentId);

        try
        {
            var response = await _apiService.PostAsync<EndorsementDto?>("api/collaboration/toggle", request);

            string action = response != null ? "Endorsement criado" : "Endorsement removido";
            LogToggleEndorsementSuccess(_logger, action);

            return response;
        }
        catch (Exception ex)
        {
            LogToggleEndorsementError(_logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<EndorsementDto?> CheckUserEndorsementAsync(Guid? postId = null, Guid? commentId = null, Guid? userId = null)
    {
        LogCheckUserEndorsement(_logger, postId, commentId, userId);

        try
        {
            var queryParams = BuildQueryParams(new Dictionary<string, object?>
            {
                ["postId"] = postId,
                ["commentId"] = commentId,
                ["userId"] = userId
            });

            var response = await _apiService.GetAsync<EndorsementDto?>($"api/collaboration/check{queryParams}");

            bool hasEndorsement = response != null;
            LogCheckUserEndorsementSuccess(_logger, hasEndorsement);

            return response;
        }
        catch (Exception ex)
        {
            LogCheckUserEndorsementError(_logger, ex.Message, ex);
            return null;
        }
    }

    /// <summary>
    /// Helper para construir query parameters
    /// </summary>
    private static string BuildQueryParams(Dictionary<string, object?> parameters)
    {
        var queryParams = parameters
            .Where(p => p.Value != null)
            .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value!.ToString()!)}")
            .ToList();

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }

    /// <summary>
    /// Dados de fallback para teste e desenvolvimento
    /// </summary>
    private static PagedResult<EndorsementDto> GetFallbackEndorsements(EndorsementSearchRequest request)
    {
        var mockEndorsements = new List<EndorsementDto>
        {
            new() {
                Id = Guid.NewGuid(),
                Type = EndorsementType.Helpful,
                TypeDisplayName = "Útil",
                TypeIcon = "👍",
                Note = "Muito útil para entender o processo",
                IsPublic = true,
                EndorsedAt = DateTime.UtcNow.AddHours(-2),
                Context = "Processo de onboarding",
                EndorserId = Guid.NewGuid(),
                EndorserName = "Ana Silva",
                EndorserEmail = "ana.silva@synqcore.com",
                EndorserDepartment = "Recursos Humanos",
                EndorserPosition = "Especialista em RH",
                PostId = Guid.NewGuid(),
                PostTitle = "Guia de Onboarding 2024"
            },
            new() {
                Id = Guid.NewGuid(),
                Type = EndorsementType.Insightful,
                TypeDisplayName = "Perspicaz",
                TypeIcon = "💡",
                Note = "Insight valioso sobre market trends",
                IsPublic = true,
                EndorsedAt = DateTime.UtcNow.AddHours(-5),
                Context = "Análise de mercado",
                EndorserId = Guid.NewGuid(),
                EndorserName = "Carlos Santos",
                EndorserEmail = "carlos.santos@synqcore.com",
                EndorserDepartment = "Estratégia",
                EndorserPosition = "Analista Sênior",
                PostId = Guid.NewGuid(),
                PostTitle = "Tendências de Mercado Q3 2024"
            },
            new() {
                Id = Guid.NewGuid(),
                Type = EndorsementType.Accurate,
                TypeDisplayName = "Preciso",
                TypeIcon = "🎯",
                Note = "Dados extremamente precisos",
                IsPublic = true,
                EndorsedAt = DateTime.UtcNow.AddHours(-8),
                Context = "Relatório financeiro",
                EndorserId = Guid.NewGuid(),
                EndorserName = "Maria Oliveira",
                EndorserEmail = "maria.oliveira@synqcore.com",
                EndorserDepartment = "Financeiro",
                EndorserPosition = "Controladora",
                CommentId = Guid.NewGuid(),
                CommentContent = "Análise detalhada dos indicadores"
            }
        };

        return new PagedResult<EndorsementDto>
        {
            Items = mockEndorsements.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
            TotalCount = mockEndorsements.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static List<EndorsementDto> GetFallbackPostEndorsements(Guid postId)
    {
        return new List<EndorsementDto>
        {
            new() {
                Id = Guid.NewGuid(),
                Type = EndorsementType.Helpful,
                TypeDisplayName = "Útil",
                TypeIcon = "👍",
                IsPublic = true,
                EndorsedAt = DateTime.UtcNow.AddHours(-1),
                EndorserId = Guid.NewGuid(),
                EndorserName = "João Silva",
                EndorserEmail = "joao.silva@synqcore.com",
                PostId = postId
            },
            new() {
                Id = Guid.NewGuid(),
                Type = EndorsementType.Insightful,
                TypeDisplayName = "Perspicaz",
                TypeIcon = "💡",
                IsPublic = true,
                EndorsedAt = DateTime.UtcNow.AddHours(-3),
                EndorserId = Guid.NewGuid(),
                EndorserName = "Ana Costa",
                EndorserEmail = "ana.costa@synqcore.com",
                PostId = postId
            }
        };
    }

    private static EndorsementStatsDto GetFallbackStats(Guid? postId, Guid? commentId)
    {
        return new EndorsementStatsDto
        {
            ContentId = postId ?? commentId ?? Guid.NewGuid(),
            ContentType = postId.HasValue ? "Post" : "Comment",
            TotalEndorsements = 15,
            HelpfulCount = 6,
            InsightfulCount = 4,
            AccurateCount = 2,
            InnovativeCount = 2,
            ComprehensiveCount = 1,
            WellResearchedCount = 0,
            ActionableCount = 0,
            StrategicCount = 0,
            TopEndorsementType = EndorsementType.Helpful,
            TopEndorsementTypeIcon = "👍"
        };
    }

    private static EndorsementAnalyticsDto GetFallbackAnalytics()
    {
        return new EndorsementAnalyticsDto
        {
            PeriodStart = DateTime.UtcNow.AddDays(-30),
            PeriodEnd = DateTime.UtcNow,
            TotalEndorsements = 245,
            TotalParticipants = 42,
            TotalContentEndorsed = 38,
            EndorsementsByType = new Dictionary<EndorsementType, int>
            {
                [EndorsementType.Helpful] = 98,
                [EndorsementType.Insightful] = 67,
                [EndorsementType.Accurate] = 45,
                [EndorsementType.Innovative] = 23,
                [EndorsementType.Comprehensive] = 12
            },
            EndorsementsByDepartment = new Dictionary<string, int>
            {
                ["Tecnologia"] = 89,
                ["Recursos Humanos"] = 56,
                ["Financeiro"] = 34,
                ["Marketing"] = 28,
                ["Estratégia"] = 23,
                ["Vendas"] = 15
            },
            TopEndorsersGiven = [],
            TopEndorsersReceived = [],
            GrowthRate = 15.8,
            MostPopularType = EndorsementType.Helpful,
            MostActiveDay = "Terça-feira",
            AverageEndorsementsPerEmployee = 5.8,
            EndorsementRate = 0.73
        };
    }
}
