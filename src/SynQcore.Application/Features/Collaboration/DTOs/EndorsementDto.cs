using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.DTOs;

/// <summary>
/// DTO para visualização de endorsement corporativo.
/// Representa o reconhecimento de qualidade de conteúdo por colegas.
/// </summary>
public class EndorsementDto
{
    /// <summary>
    /// Identificador único do endorsement.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tipo do endorsement (Helpful, Insightful, etc.).
    /// </summary>
    public EndorsementType Type { get; set; }

    /// <summary>
    /// Nome amigável do tipo de endorsement.
    /// </summary>
    public string TypeDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Ícone representativo do tipo de endorsement.
    /// </summary>
    public string TypeIcon { get; set; } = string.Empty;

    /// <summary>
    /// Nota opcional explicando o endorsement.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Indica se o endorsement é público na organização.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Data e hora em que o endorsement foi dado.
    /// </summary>
    public DateTime EndorsedAt { get; set; }

    /// <summary>
    /// Contexto adicional do endorsement.
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// ID do funcionário que deu o endorsement.
    /// </summary>
    public Guid EndorserId { get; set; }

    /// <summary>
    /// Nome completo do funcionário que deu o endorsement.
    /// </summary>
    public string EndorserName { get; set; } = string.Empty;

    /// <summary>
    /// Email do funcionário que deu o endorsement.
    /// </summary>
    public string EndorserEmail { get; set; } = string.Empty;

    /// <summary>
    /// Departamento do funcionário que deu o endorsement.
    /// </summary>
    public string? EndorserDepartment { get; set; }

    /// <summary>
    /// Cargo do funcionário que deu o endorsement.
    /// </summary>
    public string? EndorserPosition { get; set; }

    /// <summary>
    /// ID do post endossado (se aplicável).
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// Título do post endossado.
    /// </summary>
    public string? PostTitle { get; set; }

    /// <summary>
    /// ID do comentário endossado (se aplicável).
    /// </summary>
    public Guid? CommentId { get; set; }

    /// <summary>
    /// Conteúdo do comentário endossado.
    /// </summary>
    public string? CommentContent { get; set; }
}

/// <summary>
/// DTO para criar novo endorsement corporativo.
/// Define os dados necessários para reconhecer a qualidade de um conteúdo.
/// </summary>
public class CreateEndorsementDto
{
    /// <summary>
    /// ID do post a ser endossado (obrigatório se CommentId for nulo).
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// ID do comentário a ser endossado (obrigatório se PostId for nulo).
    /// </summary>
    public Guid? CommentId { get; set; }

    /// <summary>
    /// Tipo de endorsement a ser concedido.
    /// </summary>
    public EndorsementType Type { get; set; }

    /// <summary>
    /// Nota opcional explicando o endorsement.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Define se o endorsement será visível publicamente.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Contexto adicional do endorsement.
    /// </summary>
    public string? Context { get; set; }
}

/// <summary>
/// DTO para atualizar endorsement existente.
/// Permite modificar propriedades específicas do endorsement.
/// </summary>
public class UpdateEndorsementDto
{
    /// <summary>
    /// Novo tipo de endorsement (opcional).
    /// </summary>
    public EndorsementType? Type { get; set; }

    /// <summary>
    /// Nova nota explicativa (opcional).
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Nova configuração de visibilidade (opcional).
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>
    /// Novo contexto do endorsement (opcional).
    /// </summary>
    public string? Context { get; set; }
}

/// <summary>
/// DTO para busca avançada de endorsements com filtros e paginação.
/// Permite filtrar endorsements por vários critérios.
/// </summary>
public class EndorsementSearchDto
{
    /// <summary>
    /// Filtrar endorsements de um post específico.
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// Filtrar endorsements de um comentário específico.
    /// </summary>
    public Guid? CommentId { get; set; }

    /// <summary>
    /// Filtrar endorsements dados por um funcionário específico.
    /// </summary>
    public Guid? EndorserId { get; set; }

    /// <summary>
    /// Filtrar por tipo específico de endorsement.
    /// </summary>
    public EndorsementType? Type { get; set; }

    /// <summary>
    /// Filtrar por contexto específico.
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// Filtrar apenas endorsements públicos ou privados.
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>
    /// Filtrar endorsements criados após esta data.
    /// </summary>
    public DateTime? EndorsedAfter { get; set; }

    /// <summary>
    /// Filtrar endorsements criados antes desta data.
    /// </summary>
    public DateTime? EndorsedBefore { get; set; }

    /// <summary>
    /// Campo para ordenação dos resultados.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Indica se a ordenação é descendente.
    /// </summary>
    public bool SortDescending { get; set; }

    /// <summary>
    /// Número da página para paginação.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Tamanho da página para paginação.
    /// </summary>
    public int PageSize { get; set; }
}

/// <summary>
/// DTO para estatísticas de endorsements por conteúdo específico.
/// Apresenta métricas detalhadas de reconhecimento.
/// </summary>
public class EndorsementStatsDto
{
    /// <summary>
    /// ID do conteúdo (post ou comentário) analisado.
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Tipo do conteúdo ("Post" ou "Comment").
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Total de endorsements recebidos pelo conteúdo.
    /// </summary>
    public int TotalEndorsements { get; set; }

    /// <summary>
    /// Quantidade de endorsements do tipo "Helpful".
    /// </summary>
    public int HelpfulCount { get; set; }

    /// <summary>
    /// Quantidade de endorsements do tipo "Insightful".
    /// </summary>
    public int InsightfulCount { get; set; }

    /// <summary>
    /// Quantidade de endorsements do tipo "Accurate".
    /// </summary>
    public int AccurateCount { get; set; }

    /// <summary>
    /// Quantidade de endorsements do tipo "Innovative".
    /// </summary>
    public int InnovativeCount { get; set; }

    /// <summary>
    /// Quantidade de endorsements do tipo "Comprehensive".
    /// </summary>
    public int ComprehensiveCount { get; set; }

    /// <summary>
    /// Quantidade de endorsements do tipo "WellResearched".
    /// </summary>
    public int WellResearchedCount { get; set; }

    /// <summary>
    /// Quantidade de endorsements do tipo "Actionable".
    /// </summary>
    public int ActionableCount { get; set; }

    /// <summary>
    /// Quantidade de endorsements do tipo "Strategic".
    /// </summary>
    public int StrategicCount { get; set; }

    /// <summary>
    /// Tipo de endorsement mais comum para este conteúdo.
    /// </summary>
    public EndorsementType? TopEndorsementType { get; set; }

    /// <summary>
    /// Ícone do tipo de endorsement mais comum.
    /// </summary>
    public string? TopEndorsementTypeIcon { get; set; }
}

/// <summary>
/// DTO para ranking de funcionários baseado em endorsements.
/// Apresenta métricas de engajamento e posicionamento.
/// </summary>
public class EmployeeEndorsementRankingDto
{
    /// <summary>
    /// ID do funcionário no ranking.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Nome completo do funcionário.
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Email corporativo do funcionário.
    /// </summary>
    public string EmployeeEmail { get; set; } = string.Empty;

    /// <summary>
    /// Departamento do funcionário.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Cargo do funcionário.
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Total de endorsements recebidos pelo funcionário.
    /// </summary>
    public int TotalEndorsementsReceived { get; set; }

    /// <summary>
    /// Total de endorsements dados pelo funcionário.
    /// </summary>
    public int TotalEndorsementsGiven { get; set; }

    /// <summary>
    /// Endorsements "Helpful" recebidos.
    /// </summary>
    public int HelpfulReceived { get; set; }

    /// <summary>
    /// Endorsements "Insightful" recebidos.
    /// </summary>
    public int InsightfulReceived { get; set; }

    /// <summary>
    /// Endorsements "Accurate" recebidos.
    /// </summary>
    public int AccurateReceived { get; set; }

    /// <summary>
    /// Endorsements "Innovative" recebidos.
    /// </summary>
    public int InnovativeReceived { get; set; }

    /// <summary>
    /// Score de engajamento calculado por fórmula corporativa.
    /// </summary>
    public double EngagementScore { get; set; }

    /// <summary>
    /// Posição do funcionário no ranking geral.
    /// </summary>
    public int Ranking { get; set; }
}

/// <summary>
/// DTO para analytics gerais de endorsements corporativos.
/// Fornece visão abrangente das métricas de reconhecimento.
/// </summary>
public class EndorsementAnalyticsDto
{
    /// <summary>
    /// Data de início do período analisado.
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Data de fim do período analisado.
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// Filtro de departamento aplicado (se houver).
    /// </summary>
    public string? DepartmentFilter { get; set; }

    /// <summary>
    /// Total de endorsements no período.
    /// </summary>
    public int TotalEndorsements { get; set; }

    /// <summary>
    /// Total de funcionários participantes.
    /// </summary>
    public int TotalParticipants { get; set; }

    /// <summary>
    /// Total de conteúdos que receberam endorsements.
    /// </summary>
    public int TotalContentEndorsed { get; set; }

    /// <summary>
    /// Distribuição de endorsements por tipo.
    /// </summary>
    public Dictionary<EndorsementType, int> EndorsementsByType { get; set; } = [];

    /// <summary>
    /// Distribuição de endorsements por departamento.
    /// </summary>
    public Dictionary<string, int> EndorsementsByDepartment { get; set; } = [];

    /// <summary>
    /// Ranking dos funcionários que mais dão endorsements.
    /// </summary>
    public List<EmployeeEndorsementRankingDto> TopEndorsersGiven { get; set; } = [];

    /// <summary>
    /// Ranking dos funcionários que mais recebem endorsements.
    /// </summary>
    public List<EmployeeEndorsementRankingDto> TopEndorsersReceived { get; set; } = [];

    /// <summary>
    /// Taxa de crescimento de endorsements no período.
    /// </summary>
    public double GrowthRate { get; set; }

    /// <summary>
    /// Tipo de endorsement mais popular no período.
    /// </summary>
    public EndorsementType MostPopularType { get; set; }

    /// <summary>
    /// Dia da semana com maior atividade de endorsements.
    /// </summary>
    public string MostActiveDay { get; set; } = string.Empty;

    /// <summary>
    /// Média de endorsements por funcionário.
    /// </summary>
    public double AverageEndorsementsPerEmployee { get; set; }

    /// <summary>
    /// Taxa de endorsement (total endorsements / total posts+comentários).
    /// </summary>
    public double EndorsementRate { get; set; }
}
