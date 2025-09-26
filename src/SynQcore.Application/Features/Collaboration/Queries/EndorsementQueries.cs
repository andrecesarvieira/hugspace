using MediatR;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Common.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.Queries;

// Query para buscar endorsements com filtros
public record GetEndorsementsQuery : IRequest<PagedResult<EndorsementDto>>
{
    public EndorsementSearchDto SearchRequest { get; set; } = null!;
}

// Query para obter endorsement específico por ID
public record GetEndorsementByIdQuery : IRequest<EndorsementDto>
{
    public Guid Id { get; set; }
}

// Query para obter endorsements de um post específico
public record GetPostEndorsementsQuery : IRequest<List<EndorsementDto>>
{
    public Guid PostId { get; set; }
    public EndorsementType? FilterByType { get; set; }
    public bool IncludePrivate { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

// Query para obter endorsements de um comentário específico
public record GetCommentEndorsementsQuery : IRequest<List<EndorsementDto>>
{
    public Guid CommentId { get; set; }
    public EndorsementType? FilterByType { get; set; }
    public bool IncludePrivate { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

// Query para obter estatísticas de endorsements de um conteúdo
public record GetEndorsementStatsQuery : IRequest<EndorsementStatsDto>
{
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    public bool IncludePrivate { get; set; }
}

// Query para ranking de funcionários por endorsements
public record GetEndorsementAnalyticsQuery : IRequest<EndorsementAnalyticsDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? SearchTerm { get; set; }
    public bool IncludePrivate { get; set; }
}

// Query para obter endorsements dados por um funcionário
public record GetEmployeeEndorsementsGivenQuery : IRequest<PagedResult<EndorsementDto>>
{
    public Guid EmployeeId { get; set; }
    public EndorsementType? FilterByType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

// Query para obter endorsements recebidos por um funcionário
public record GetEmployeeEndorsementsReceivedQuery : IRequest<PagedResult<EndorsementDto>>
{
    public Guid EmployeeId { get; set; }
    public EndorsementType? FilterByType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

// Query para verificar se usuário já endossou um conteúdo específico
public record CheckUserEndorsementQuery : IRequest<EndorsementDto?>
{
    public Guid UserId { get; set; }
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    public EndorsementType? SpecificType { get; set; }
}

// Query para obter tipos de endorsement mais populares por período
public record GetTrendingEndorsementTypesQuery : IRequest<List<EndorsementTypeTrendDto>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public int TopCount { get; set; }
}

// DTO para trending de tipos de endorsement
public class EndorsementTypeTrendDto
{
    public EndorsementType Type { get; set; }
    public string TypeDisplayName { get; set; } = string.Empty;
    public string TypeIcon { get; set; } = string.Empty;
    public int Count { get; set; }
    public double PercentageOfTotal { get; set; }
    public double GrowthRate { get; set; } // Comparado com período anterior
}

// Query para ranking de funcionários por endorsements recebidos
public record GetEmployeeEndorsementRankingQuery : IRequest<List<EmployeeEndorsementRankingDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public int TopCount { get; set; }
    public string? SearchTerm { get; set; }
    public string RankingType { get; set; } = "engagement"; // received, given, engagement
}