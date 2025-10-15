/*
 * SynQcore - Corporate Social Network
 *
 * Interface de Serviço para Gestão de Endorsements
 * Implementa padrão de validação de conhecimento entre pares corporativos
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using System.ComponentModel.DataAnnotations;
using SynQcore.BlazorApp.Models;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Interface para gerenciamento de endorsements corporativos
/// Fornece métodos para validação de conhecimento entre pares
/// </summary>
public interface IEndorsementService
{
    /// <summary>
    /// Buscar endorsements com filtros e paginação
    /// </summary>
    /// <param name="searchRequest">Critérios de busca</param>
    /// <returns>Lista paginada de endorsements</returns>
    Task<PagedResult<EndorsementDto>> SearchEndorsementsAsync(EndorsementSearchRequest searchRequest);

    /// <summary>
    /// Obter endorsement específico por ID
    /// </summary>
    /// <param name="id">ID do endorsement</param>
    /// <returns>Detalhes do endorsement ou null se não encontrado</returns>
    Task<EndorsementDto?> GetEndorsementByIdAsync(Guid id);

    /// <summary>
    /// Obter endorsements de um post específico
    /// </summary>
    /// <param name="postId">ID do post</param>
    /// <param name="filterByType">Tipo de endorsement para filtrar (opcional)</param>
    /// <param name="includePrivate">Incluir endorsements privados</param>
    /// <returns>Lista de endorsements do post</returns>
    Task<List<EndorsementDto>> GetPostEndorsementsAsync(Guid postId, EndorsementType? filterByType = null, bool includePrivate = false);

    /// <summary>
    /// Obter estatísticas de endorsements de um conteúdo
    /// </summary>
    /// <param name="postId">ID do post (opcional)</param>
    /// <param name="commentId">ID do comentário (opcional)</param>
    /// <param name="includePrivate">Incluir endorsements privados</param>
    /// <returns>Estatísticas de endorsements</returns>
    Task<EndorsementStatsDto> GetEndorsementStatsAsync(Guid? postId = null, Guid? commentId = null, bool includePrivate = false);

    /// <summary>
    /// Obter analytics de endorsements corporativos
    /// </summary>
    /// <param name="startDate">Data inicial do período</param>
    /// <param name="endDate">Data final do período</param>
    /// <param name="departmentId">ID do departamento para filtrar</param>
    /// <param name="includePrivate">Incluir endorsements privados</param>
    /// <returns>Analytics completo de endorsements</returns>
    Task<EndorsementAnalyticsDto> GetEndorsementAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null, Guid? departmentId = null, bool includePrivate = false);

    /// <summary>
    /// Criar um novo endorsement
    /// </summary>
    /// <param name="request">Dados do endorsement a ser criado</param>
    /// <returns>Endorsement criado</returns>
    Task<EndorsementDto> CreateEndorsementAsync(CreateEndorsementRequest request);

    /// <summary>
    /// Atualizar um endorsement existente
    /// </summary>
    /// <param name="id">ID do endorsement</param>
    /// <param name="request">Dados atualizados</param>
    /// <returns>Endorsement atualizado</returns>
    Task<EndorsementDto> UpdateEndorsementAsync(Guid id, UpdateEndorsementRequest request);

    /// <summary>
    /// Excluir um endorsement
    /// </summary>
    /// <param name="id">ID do endorsement</param>
    /// <param name="reason">Motivo da exclusão</param>
    /// <returns>True se excluído com sucesso</returns>
    Task<bool> DeleteEndorsementAsync(Guid id, string? reason = null);

    /// <summary>
    /// Toggle (criar/remover) endorsement em um conteúdo
    /// </summary>
    /// <param name="request">Dados do toggle</param>
    /// <returns>Endorsement resultante ou null se removido</returns>
    Task<EndorsementDto?> ToggleEndorsementAsync(ToggleEndorsementRequest request);

    /// <summary>
    /// Verificar se usuário já endossou um conteúdo
    /// </summary>
    /// <param name="postId">ID do post (opcional)</param>
    /// <param name="commentId">ID do comentário (opcional)</param>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Endorsement do usuário se existir</returns>
    Task<EndorsementDto?> CheckUserEndorsementAsync(Guid? postId = null, Guid? commentId = null, Guid? userId = null);
}

/// <summary>
/// DTO para criação de endorsement
/// </summary>
public class CreateEndorsementRequest
{
    [Required(ErrorMessage = "É necessário especificar o tipo de endorsement")]
    public EndorsementType Type { get; set; }

    public Guid? PostId { get; set; }

    public Guid? CommentId { get; set; }

    [StringLength(1000, ErrorMessage = "A nota deve ter no máximo 1000 caracteres")]
    public string? Note { get; set; }

    public bool IsPublic { get; set; } = true;

    [StringLength(500, ErrorMessage = "O contexto deve ter no máximo 500 caracteres")]
    public string? Context { get; set; }
}

/// <summary>
/// DTO para atualização de endorsement
/// </summary>
public class UpdateEndorsementRequest
{
    public EndorsementType? Type { get; set; }

    [StringLength(1000, ErrorMessage = "A nota deve ter no máximo 1000 caracteres")]
    public string? Note { get; set; }

    public bool? IsPublic { get; set; }

    [StringLength(500, ErrorMessage = "O contexto deve ter no máximo 500 caracteres")]
    public string? Context { get; set; }
}

/// <summary>
/// DTO para busca de endorsements
/// </summary>
public class EndorsementSearchRequest
{
    public Guid? PostId { get; set; }

    public Guid? CommentId { get; set; }

    public Guid? EndorserId { get; set; }

    public EndorsementType? Type { get; set; }

    [StringLength(500, ErrorMessage = "O contexto deve ter no máximo 500 caracteres")]
    public string? Context { get; set; }

    public bool? IsPublic { get; set; }

    public DateTime? EndorsedAfter { get; set; }

    public DateTime? EndorsedBefore { get; set; }

    [StringLength(50, ErrorMessage = "O campo de ordenação deve ter no máximo 50 caracteres")]
    public string? SortBy { get; set; } = "EndorsedAt";

    public bool SortDescending { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior que 0")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// DTO para toggle de endorsement
/// </summary>
public class ToggleEndorsementRequest
{
    [Required(ErrorMessage = "É necessário especificar o tipo de endorsement")]
    public EndorsementType Type { get; set; }

    public Guid? PostId { get; set; }

    public Guid? CommentId { get; set; }

    [StringLength(1000, ErrorMessage = "A nota deve ter no máximo 1000 caracteres")]
    public string? Note { get; set; }

    public bool IsPublic { get; set; } = true;

    [StringLength(500, ErrorMessage = "O contexto deve ter no máximo 500 caracteres")]
    public string? Context { get; set; }
}

/// <summary>
/// DTOs para resposta da API (mapeados dos DTOs do backend)
/// </summary>
public class EndorsementDto
{
    public Guid Id { get; set; }
    public EndorsementType Type { get; set; }
    public string TypeDisplayName { get; set; } = string.Empty;
    public string TypeIcon { get; set; } = string.Empty;
    public string? Note { get; set; }
    public bool IsPublic { get; set; }
    public DateTime EndorsedAt { get; set; }
    public string? Context { get; set; }
    public Guid EndorserId { get; set; }
    public string EndorserName { get; set; } = string.Empty;
    public string EndorserEmail { get; set; } = string.Empty;
    public string? EndorserDepartment { get; set; }
    public string? EndorserPosition { get; set; }
    public Guid? PostId { get; set; }
    public string? PostTitle { get; set; }
    public Guid? CommentId { get; set; }
    public string? CommentContent { get; set; }
}

public class EndorsementStatsDto
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public int TotalEndorsements { get; set; }
    public int HelpfulCount { get; set; }
    public int InsightfulCount { get; set; }
    public int AccurateCount { get; set; }
    public int InnovativeCount { get; set; }
    public int ComprehensiveCount { get; set; }
    public int WellResearchedCount { get; set; }
    public int ActionableCount { get; set; }
    public int StrategicCount { get; set; }
    public EndorsementType? TopEndorsementType { get; set; }
    public string? TopEndorsementTypeIcon { get; set; }
}

public class EmployeeEndorsementRankingDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public int TotalEndorsementsReceived { get; set; }
    public int TotalEndorsementsGiven { get; set; }
    public int HelpfulReceived { get; set; }
    public int InsightfulReceived { get; set; }
    public int AccurateReceived { get; set; }
    public int InnovativeReceived { get; set; }
    public double EngagementScore { get; set; }
    public int Ranking { get; set; }
}

public class EndorsementAnalyticsDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string? DepartmentFilter { get; set; }
    public int TotalEndorsements { get; set; }
    public int TotalParticipants { get; set; }
    public int TotalContentEndorsed { get; set; }
    public Dictionary<EndorsementType, int> EndorsementsByType { get; set; } = [];
    public Dictionary<string, int> EndorsementsByDepartment { get; set; } = [];
    public List<EmployeeEndorsementRankingDto> TopEndorsersGiven { get; set; } = [];
    public List<EmployeeEndorsementRankingDto> TopEndorsersReceived { get; set; } = [];
    public double GrowthRate { get; set; }
    public EndorsementType MostPopularType { get; set; }
    public string MostActiveDay { get; set; } = string.Empty;
    public double AverageEndorsementsPerEmployee { get; set; }
    public double EndorsementRate { get; set; }
}
