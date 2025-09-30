using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;

namespace SynQcore.Application.Features.Employees.Handlers;

public partial class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetEmployeeByIdHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 4011, Level = LogLevel.Information,
        Message = "Buscando funcionário por ID: {EmployeeId}")]
    private static partial void LogBuscandoFuncionario(ILogger logger, Guid employeeId);

    [LoggerMessage(EventId = 4012, Level = LogLevel.Information,
        Message = "Funcionário encontrado: {EmployeeId} - {Nome}")]
    private static partial void LogFuncionarioEncontrado(ILogger logger, Guid employeeId, string nome);

    [LoggerMessage(EventId = 4013, Level = LogLevel.Warning,
        Message = "Funcionário não encontrado: {EmployeeId}")]
    private static partial void LogFuncionarioNaoEncontrado(ILogger logger, Guid employeeId);

    [LoggerMessage(EventId = 4014, Level = LogLevel.Error,
        Message = "Erro ao buscar funcionário: {EmployeeId}")]
    private static partial void LogErroFuncionario(ILogger logger, Guid employeeId, Exception ex);

    public GetEmployeeByIdHandler(ISynQcoreDbContext context, ILogger<GetEmployeeByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            LogBuscandoFuncionario(_logger, request.Id);

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .Include(e => e.EmployeeDepartments.Where(ed => !ed.IsDeleted))
                    .ThenInclude(ed => ed.Department)
                .Include(e => e.TeamMemberships.Where(tm => !tm.IsDeleted))
                    .ThenInclude(tm => tm.Team)
                .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

            if (employee == null)
            {
                LogFuncionarioNaoEncontrado(_logger, request.Id);
                throw new NotFoundException($"Employee with ID {request.Id} not found");
            }

            var nomeCompleto = $"{employee.FirstName} {employee.LastName}";
            LogFuncionarioEncontrado(_logger, employee.Id, nomeCompleto);

            return employee.ToEmployeeDto();
        }
        catch (Exception ex) when (!(ex is NotFoundException))
        {
            LogErroFuncionario(_logger, request.Id, ex);
            throw;
        }
    }
}
