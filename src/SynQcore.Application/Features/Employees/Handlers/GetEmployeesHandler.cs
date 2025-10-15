using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;

namespace SynQcore.Application.Features.Employees.Handlers;

public partial class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetEmployeesHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 4001, Level = LogLevel.Information,
        Message = "Buscando funcionários - Página: {Page}, Tamanho: {PageSize}, Filtros: {HasFilters}")]
    private static partial void LogBuscandoFuncionarios(ILogger logger, int page, int pageSize, bool hasFilters);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Information,
        Message = "Funcionários encontrados: {Count} resultados, Total: {Total}")]
    private static partial void LogFuncionariosEncontrados(ILogger logger, int count, int total);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Error,
        Message = "Erro ao buscar funcionários")]
    private static partial void LogErroFuncionarios(ILogger logger, Exception ex);

    public GetEmployeesHandler(ISynQcoreDbContext context, ILogger<GetEmployeesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var hasFilters = !string.IsNullOrEmpty(request.Request.SearchTerm) ||
                           request.Request.DepartmentId.HasValue ||
                           request.Request.TeamId.HasValue ||
                           request.Request.ManagerId.HasValue ||
                           request.Request.IsActive.HasValue;

            LogBuscandoFuncionarios(_logger, request.Request.Page, request.Request.PageSize, hasFilters);

            var query = _context.Employees
                .Include(e => e.Manager)
                .Include(e => e.EmployeeDepartments.Where(ed => !ed.IsDeleted))
                    .ThenInclude(ed => ed.Department)
                .Include(e => e.TeamMemberships.Where(tm => !tm.IsDeleted))
                    .ThenInclude(tm => tm.Team)
                .Where(e => !e.IsDeleted)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.Request.SearchTerm))
            {
                var searchTerm = request.Request.SearchTerm;
                query = query.Where(e =>
                    e.FirstName.Contains(searchTerm) ||
                    e.LastName.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm));
            }

            if (request.Request.DepartmentId.HasValue)
            {
                query = query.Where(e => e.EmployeeDepartments
                    .Any(ed => ed.DepartmentId == request.Request.DepartmentId.Value && !ed.IsDeleted));
            }

            if (request.Request.TeamId.HasValue)
            {
                query = query.Where(e => e.TeamMemberships
                    .Any(tm => tm.TeamId == request.Request.TeamId.Value && !tm.IsDeleted));
            }

            if (request.Request.ManagerId.HasValue)
            {
                query = query.Where(e => e.ManagerId == request.Request.ManagerId.Value);
            }

            if (request.Request.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == request.Request.IsActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var employees = await query
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Skip((request.Request.Page - 1) * request.Request.PageSize)
                .Take(request.Request.PageSize)
                .ToListAsync(cancellationToken);

            var employeeDtos = employees.ToEmployeeDtos();

            var result = new PagedResult<EmployeeDto>
            {
                Items = employeeDtos,
                TotalCount = totalCount,
                Page = request.Request.Page,
                PageSize = request.Request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.Request.PageSize)
            };

            LogFuncionariosEncontrados(_logger, employeeDtos.Count, totalCount);
            return result;
        }
        catch (Exception ex)
        {
            LogErroFuncionarios(_logger, ex);
            throw;
        }
    }
}
