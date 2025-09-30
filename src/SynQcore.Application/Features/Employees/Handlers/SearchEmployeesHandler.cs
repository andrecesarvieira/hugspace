using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;

namespace SynQcore.Application.Features.Employees.Handlers;

public partial class SearchEmployeesHandler : IRequestHandler<SearchEmployeesQuery, List<EmployeeDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<SearchEmployeesHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 4021, Level = LogLevel.Information,
        Message = "Pesquisando funcionários com termo: '{SearchTerm}'")]
    private static partial void LogPesquisandoFuncionarios(ILogger logger, string searchTerm);

    [LoggerMessage(EventId = 4022, Level = LogLevel.Information,
        Message = "Pesquisa concluída: {Count} funcionários encontrados para '{SearchTerm}'")]
    private static partial void LogPesquisaConcluida(ILogger logger, int count, string searchTerm);

    [LoggerMessage(EventId = 4023, Level = LogLevel.Warning,
        Message = "Termo de pesquisa vazio ou inválido")]
    private static partial void LogTermoPesquisaInvalido(ILogger logger);

    [LoggerMessage(EventId = 4024, Level = LogLevel.Error,
        Message = "Erro ao pesquisar funcionários com termo: '{SearchTerm}'")]
    private static partial void LogErroPesquisa(ILogger logger, string searchTerm, Exception ex);

    public SearchEmployeesHandler(ISynQcoreDbContext context, ILogger<SearchEmployeesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<EmployeeDto>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            LogTermoPesquisaInvalido(_logger);
            return new List<EmployeeDto>();
        }

        try
        {
            LogPesquisandoFuncionarios(_logger, request.SearchTerm);

            var searchTerm = request.SearchTerm.ToLower(CultureInfo.InvariantCulture);

            var employees = await _context.Employees
                .Where(e => e.FirstName.Contains(request.SearchTerm) ||
                           e.LastName.Contains(request.SearchTerm) ||
                           e.Email.Contains(request.SearchTerm) ||
                           e.JobTitle.Contains(request.SearchTerm))
                .ToListAsync(cancellationToken);

            var result = employees.ToEmployeeDtos();
            LogPesquisaConcluida(_logger, result.Count, request.SearchTerm);

            return result;
        }
        catch (Exception ex)
        {
            LogErroPesquisa(_logger, request.SearchTerm, ex);
            throw;
        }
    }
}
