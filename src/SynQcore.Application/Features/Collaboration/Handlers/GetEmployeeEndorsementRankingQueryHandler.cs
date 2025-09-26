using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Queries;

namespace SynQcore.Application.Features.Collaboration.Handlers;

/// Handler simplificado para ranking de funcionários por endorsements
public partial class GetEmployeeEndorsementRankingQueryHandler : IRequestHandler<GetEmployeeEndorsementRankingQuery, List<EmployeeEndorsementRankingDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetEmployeeEndorsementRankingQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 6001, Level = LogLevel.Information,
        Message = "Calculando ranking de funcionários por endorsements - Tipo: {RankingType}, Top: {TopCount}")]
    private static partial void LogCalculatingRanking(ILogger logger, string rankingType, int topCount);

    [LoggerMessage(EventId = 6002, Level = LogLevel.Information,
        Message = "Ranking gerado com {Count} funcionários")]
    private static partial void LogRankingGenerated(ILogger logger, int count);

    [LoggerMessage(EventId = 6101, Level = LogLevel.Error,
        Message = "Erro ao calcular ranking de endorsements")]
    private static partial void LogRankingError(ILogger logger, Exception ex);

    public GetEmployeeEndorsementRankingQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetEmployeeEndorsementRankingQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<EmployeeEndorsementRankingDto>> Handle(GetEmployeeEndorsementRankingQuery request, CancellationToken cancellationToken)
    {
        LogCalculatingRanking(_logger, request.RankingType, request.TopCount);

        try
        {
            // 1. Buscar funcionários com filtros
            var employeesQuery = _context.Employees
                .Include(e => e.EmployeeDepartments)
                    .ThenInclude(ed => ed.Department)
                .AsQueryable();

            if (request.DepartmentId.HasValue)
            {
                employeesQuery = employeesQuery
                    .Where(e => e.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId.Value));
            }

            var employees = await employeesQuery.ToListAsync(cancellationToken);
            
            // 2. Buscar todos os endorsements com filtros de data
            var endorsementsQuery = _context.Endorsements
                .Include(e => e.Post)
                .Include(e => e.Comment)
                .AsQueryable();

            if (request.StartDate.HasValue)
            {
                endorsementsQuery = endorsementsQuery.Where(e => e.EndorsedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                endorsementsQuery = endorsementsQuery.Where(e => e.EndorsedAt <= request.EndDate.Value);
            }

            var allEndorsements = await endorsementsQuery.ToListAsync(cancellationToken);

            // 3. Calcular estatísticas para cada funcionário
            var rankings = new List<EmployeeEndorsementRankingDto>();

            foreach (var employee in employees)
            {
                // Endorsements RECEBIDOS (conteúdo do funcionário foi endossado)
                var receivedEndorsements = allEndorsements
                    .Where(e => (e.Post?.AuthorId == employee.Id) || (e.Comment?.AuthorId == employee.Id))
                    .ToList();

                // Endorsements DADOS (funcionário endossou conteúdo de outros)
                var givenEndorsements = allEndorsements
                    .Where(e => e.EndorserId == employee.Id)
                    .ToList();

                // Contar por tipo recebido
                var helpfulReceived = receivedEndorsements.Count(e => e.Type == Domain.Entities.Communication.EndorsementType.Helpful);
                var insightfulReceived = receivedEndorsements.Count(e => e.Type == Domain.Entities.Communication.EndorsementType.Insightful);
                var accurateReceived = receivedEndorsements.Count(e => e.Type == Domain.Entities.Communication.EndorsementType.Accurate);
                var innovativeReceived = receivedEndorsements.Count(e => e.Type == Domain.Entities.Communication.EndorsementType.Innovative);

                // Calcular engagement score (fórmula corporativa)
                var engagementScore = CalculateEngagementScore(
                    totalReceived: receivedEndorsements.Count,
                    totalGiven: givenEndorsements.Count,
                    helpfulReceived,
                    insightfulReceived,
                    accurateReceived,
                    innovativeReceived
                );

                var ranking = new EmployeeEndorsementRankingDto
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName,
                    EmployeeEmail = employee.Email,
                    Department = employee.EmployeeDepartments.FirstOrDefault()?.Department?.Name ?? "Sem Departamento",
                    Position = employee.Position ?? "Não Informado",
                    
                    // Estatísticas reais
                    TotalEndorsementsReceived = receivedEndorsements.Count,
                    TotalEndorsementsGiven = givenEndorsements.Count,
                    HelpfulReceived = helpfulReceived,
                    InsightfulReceived = insightfulReceived,
                    AccurateReceived = accurateReceived,
                    InnovativeReceived = innovativeReceived,
                    EngagementScore = engagementScore,
                    Ranking = 0 // Será definido após ordenação
                };

                rankings.Add(ranking);
            }

            // 4. Aplicar ordenação por tipo de ranking
            rankings = ApplyRankingOrder(rankings, request.RankingType);

            // 5. Aplicar ranking position e limitar resultados
            var result = rankings
                .Take(request.TopCount)
                .Select((r, index) => 
                {
                    r.Ranking = index + 1;
                    return r;
                })
                .ToList();

            LogRankingGenerated(_logger, result.Count);
            return result;
        }
        catch (Exception ex)
        {
            LogRankingError(_logger, ex);
            throw;
        }
    }

    /// <summary>
    /// Calcula score de engagement baseado em métricas corporativas
    /// </summary>
    private static double CalculateEngagementScore(int totalReceived, int totalGiven, 
        int helpfulReceived, int insightfulReceived, int accurateReceived, int innovativeReceived)
    {
        if (totalReceived == 0 && totalGiven == 0) return 0.0;

        // Fórmula corporativa balanceada:
        // - Peso maior para endorsements recebidos (qualidade do conteúdo)
        // - Peso médio para endorsements dados (participação na comunidade)
        // - Bônus por diversidade de tipos recebidos
        
        var receivedScore = totalReceived * 2.0; // Peso 2x para recebidos
        var givenScore = totalGiven * 1.0; // Peso 1x para dados
        
        // Bônus por diversidade (até 4 tipos diferentes)
        var typesReceived = new[] { helpfulReceived, insightfulReceived, accurateReceived, innovativeReceived }
            .Count(count => count > 0);
        var diversityBonus = typesReceived * 0.5;
        
        // Bônus extra para tipos premium (Insightful e Innovative)
        var premiumBonus = (insightfulReceived + innovativeReceived) * 0.3;
        
        return Math.Round(receivedScore + givenScore + diversityBonus + premiumBonus, 2);
    }

    /// <summary>
    /// Aplica ordenação baseada no tipo de ranking solicitado
    /// </summary>
    private static List<EmployeeEndorsementRankingDto> ApplyRankingOrder(
        List<EmployeeEndorsementRankingDto> rankings, string rankingType)
    {
        return rankingType.ToLowerInvariant() switch
        {
            "received" => rankings.OrderByDescending(r => r.TotalEndorsementsReceived)
                                 .ThenByDescending(r => r.EngagementScore)
                                 .ToList(),
            
            "given" => rankings.OrderByDescending(r => r.TotalEndorsementsGiven)
                              .ThenByDescending(r => r.EngagementScore)
                              .ToList(),
            
            "engagement" => rankings.OrderByDescending(r => r.EngagementScore)
                                   .ThenByDescending(r => r.TotalEndorsementsReceived)
                                   .ToList(),
            
            "helpful" => rankings.OrderByDescending(r => r.HelpfulReceived)
                                .ThenByDescending(r => r.EngagementScore)
                                .ToList(),
            
            "insightful" => rankings.OrderByDescending(r => r.InsightfulReceived)
                                   .ThenByDescending(r => r.EngagementScore)
                                   .ToList(),
            
            "accurate" => rankings.OrderByDescending(r => r.AccurateReceived)
                                 .ThenByDescending(r => r.EngagementScore)
                                 .ToList(),
            
            "innovative" => rankings.OrderByDescending(r => r.InnovativeReceived)
                                   .ThenByDescending(r => r.EngagementScore)
                                   .ToList(),
            
            _ => rankings.OrderByDescending(r => r.EngagementScore) // Default: engagement
                        .ThenByDescending(r => r.TotalEndorsementsReceived)
                        .ToList()
        };
    }
}