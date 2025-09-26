using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Helpers;
using SynQcore.Application.Features.Collaboration.Queries;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.Handlers;

// Handler para ranking de funcionários baseado em endorsements corporativos
public partial class GetEmployeeEndorsementRankingQueryHandler : IRequestHandler<GetEmployeeEndorsementRankingQuery, List<EmployeeEndorsementRankingDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetEmployeeEndorsementRankingQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3061, Level = LogLevel.Information,
        Message = "Calculando ranking de funcionários por endorsements - Tipo: {RankingType}, Top: {TopCount}")]
    private static partial void LogCalculatingRanking(ILogger logger, string rankingType, int topCount);

    [LoggerMessage(EventId = 3062, Level = LogLevel.Information,
        Message = "Ranking calculado com sucesso - {EmployeeCount} funcionários processados")]
    private static partial void LogRankingCalculated(ILogger logger, int employeeCount);

    [LoggerMessage(EventId = 3063, Level = LogLevel.Information,
        Message = "Aplicando filtro por departamento: {DepartmentId}")]
    private static partial void LogDepartmentFilter(ILogger logger, Guid departmentId);

    [LoggerMessage(EventId = 3064, Level = LogLevel.Information,
        Message = "Aplicando filtro de período: {StartDate} - {EndDate}")]
    private static partial void LogDateRangeFilter(ILogger logger, DateTime startDate, DateTime endDate);

    [LoggerMessage(EventId = 3065, Level = LogLevel.Error,
        Message = "Erro ao calcular ranking de endorsements")]
    private static partial void LogRankingError(ILogger logger, Exception ex);

    public GetEmployeeEndorsementRankingQueryHandler(
        ISynQcoreDbContext context, 
        IMapper mapper, 
        ILogger<GetEmployeeEndorsementRankingQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<EmployeeEndorsementRankingDto>> Handle(GetEmployeeEndorsementRankingQuery request, CancellationToken cancellationToken)
    {
        LogCalculatingRanking(_logger, request.RankingType, request.TopCount);

        try
        {
            // Aplicar filtros de data se especificados
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                LogDateRangeFilter(_logger, request.StartDate.Value, request.EndDate.Value);
            }

            // Aplicar filtro de departamento se especificado
            if (request.DepartmentId.HasValue)
            {
                LogDepartmentFilter(_logger, request.DepartmentId.Value);
            }

            // Query base para funcionários
            var employeesQuery = _context.Employees
                .Include(e => e.EmployeeDepartments)
                    .ThenInclude(ed => ed.Department)
                .Include(e => e.Position)
                .AsQueryable();

            // Filtrar por departamento se especificado
            if (request.DepartmentId.HasValue)
            {
                employeesQuery = employeesQuery
                    .Where(e => e.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId.Value));
            }

            // Buscar endorsements com filtros de data
            var endorsementsQuery = _context.Endorsements.AsQueryable();

            if (request.StartDate.HasValue)
            {
                endorsementsQuery = endorsementsQuery.Where(e => e.EndorsedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                endorsementsQuery = endorsementsQuery.Where(e => e.EndorsedAt <= request.EndDate.Value);
            }

            // Calcular estatísticas de endorsements RECEBIDOS (baseado no autor do conteúdo)
            var receivedStats = await (from emp in employeesQuery
                                     select new
                                     {
                                         Employee = emp,
                                         // Endorsements em Posts do funcionário
                                         PostEndorsements = endorsementsQuery
                                             .Where(end => end.Post != null && end.Post.AuthorId == emp.Id)
                                             .ToList(),
                                         // Endorsements em Comments do funcionário  
                                         CommentEndorsements = endorsementsQuery
                                             .Where(end => end.Comment != null && end.Comment.AuthorId == emp.Id)
                                             .ToList()
                                     }).ToListAsync(cancellationToken);

            // Calcular estatísticas de endorsements DADOS
            var givenStats = await endorsementsQuery
                .GroupBy(e => e.EndorserId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    TotalGiven = g.Count(),
                    TypesGiven = g.GroupBy(e => e.Type).ToDictionary(tg => tg.Key, tg => tg.Count())
                })
                .ToDictionaryAsync(x => x.EmployeeId, x => x, cancellationToken);

            // Processar dados e montar ranking
            var rankings = new List<EmployeeEndorsementRankingDto>();

            foreach (var stat in receivedStats)
            {
                var employee = stat.Employee;
                var allReceivedEndorsements = stat.PostEndorsements.Concat(stat.CommentEndorsements).ToList();
                
                // Contar por tipo recebido
                var receivedByType = allReceivedEndorsements
                    .GroupBy(e => e.Type)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Estatísticas de dados
                givenStats.TryGetValue(employee.Id, out var givenStat);

                var ranking = new EmployeeEndorsementRankingDto
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName,
                    EmployeeEmail = employee.Email,
                    Department = employee.EmployeeDepartments.FirstOrDefault()?.Department.Name,
                    Position = employee.Position,
                    
                    // Endorsements recebidos
                    TotalEndorsementsReceived = allReceivedEndorsements.Count,
                    HelpfulReceived = receivedByType.GetValueOrDefault(EndorsementType.Helpful, 0),
                    InsightfulReceived = receivedByType.GetValueOrDefault(EndorsementType.Insightful, 0),
                    AccurateReceived = receivedByType.GetValueOrDefault(EndorsementType.Accurate, 0),
                    InnovativeReceived = receivedByType.GetValueOrDefault(EndorsementType.Innovative, 0),
                    
                    // Endorsements dados
                    TotalEndorsementsGiven = givenStat?.TotalGiven ?? 0
                };

                // Calcular engagement score usando helper corporativo
                var endorsementCounts = receivedByType;
                ranking.EngagementScore = EndorsementTypeHelper.CalculateEngagementScore(endorsementCounts);

                rankings.Add(ranking);
            }

            // Aplicar ordenação por tipo de ranking
            rankings = request.RankingType.ToLowerInvariant() switch
            {
                "received" => rankings.OrderByDescending(r => r.TotalEndorsementsReceived).ToList(),
                "given" => rankings.OrderByDescending(r => r.TotalEndorsementsGiven).ToList(),
                "engagement" => rankings.OrderByDescending(r => r.EngagementScore).ToList(),
                _ => rankings.OrderByDescending(r => r.EngagementScore).ToList() // Default para engagement
            };

            // Aplicar ranking numerico e limitar resultado
            for (int i = 0; i < rankings.Count && i < request.TopCount; i++)
            {
                rankings[i].Ranking = i + 1;
            }

            var finalRankings = rankings.Take(request.TopCount).ToList();

            LogRankingCalculated(_logger, finalRankings.Count);
            return finalRankings;
        }
        catch (Exception ex)
        {
            LogRankingError(_logger, ex);
            throw;
        }
    }
}