using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.Commands;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Handlers;

public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UpdateEmployeeHandler> _logger;

    private static readonly Action<ILogger, Guid, string, Exception?> LogEmployeeUpdated =
        LoggerMessage.Define<Guid, string>(LogLevel.Information, new EventId(1, "EmployeeUpdated"),
            "Employee updated successfully: {EmployeeId} - {EmployeeName}");

    public UpdateEmployeeHandler(ISynQcoreDbContext context, ILogger<UpdateEmployeeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.EmployeeDepartments.Where(ed => !ed.IsDeleted))
            .Include(e => e.TeamMemberships.Where(tm => !tm.IsDeleted))
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

        if (employee == null)
            throw new NotFoundException($"Employee with ID {request.Id} not found");

        // Validar se manager existe (se especificado)
        if (request.Request.ManagerId.HasValue && request.Request.ManagerId != employee.ManagerId)
        {
            // Verificar se não está criando uma referência circular
            if (request.Request.ManagerId == employee.Id)
                throw new ConflictException("Employee cannot be their own manager");

            var manager = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Request.ManagerId.Value && !e.IsDeleted, cancellationToken);
            
            if (manager == null)
                throw new NotFoundException($"Manager with ID {request.Request.ManagerId.Value} not found");
        }

        // Atualizar dados básicos
        employee.FirstName = request.Request.FirstName;
        employee.LastName = request.Request.LastName;
        employee.Phone = request.Request.Phone;
        employee.IsActive = request.Request.IsActive;
        employee.ManagerId = request.Request.ManagerId;
        employee.UpdateTimestamp();

        // Atualizar departamentos
        await UpdateEmployeeDepartments(employee.Id, request.Request.DepartmentIds, cancellationToken);

        // Atualizar teams
        await UpdateEmployeeTeams(employee.Id, request.Request.TeamIds, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        LogEmployeeUpdated(_logger, employee.Id, $"{employee.FirstName} {employee.LastName}", null);

        return await GetEmployeeWithRelationships(employee.Id, cancellationToken);
    }

    private async Task UpdateEmployeeDepartments(Guid employeeId, List<Guid> newDepartmentIds, CancellationToken cancellationToken)
    {
        // Remover departamentos atuais
        var currentDepartments = await _context.EmployeeDepartments
            .Where(ed => ed.EmployeeId == employeeId && !ed.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var dept in currentDepartments)
        {
            dept.MarkAsDeleted();
        }

        // Adicionar novos departamentos
        foreach (var deptId in newDepartmentIds)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == deptId && !d.IsDeleted, cancellationToken);
            
            if (department != null)
            {
                _context.EmployeeDepartments.Add(new Domain.Entities.Relationships.EmployeeDepartment
                {
                    EmployeeId = employeeId,
                    DepartmentId = deptId,
                    StartDate = DateTime.UtcNow
                });
            }
        }
    }

    private async Task UpdateEmployeeTeams(Guid employeeId, List<Guid> newTeamIds, CancellationToken cancellationToken)
    {
        // Remover teams atuais
        var currentTeams = await _context.TeamMemberships
            .Where(tm => tm.EmployeeId == employeeId && !tm.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var team in currentTeams)
        {
            team.MarkAsDeleted();
        }

        // Adicionar novos teams
        foreach (var teamId in newTeamIds)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(t => t.Id == teamId && !t.IsDeleted, cancellationToken);
            
            if (team != null)
            {
                _context.TeamMemberships.Add(new Domain.Entities.Relationships.TeamMembership
                {
                    EmployeeId = employeeId,
                    TeamId = teamId,
                    JoinedAt = DateTime.UtcNow,
                    Role = Domain.Entities.Relationships.TeamRole.Member
                });
            }
        }
    }

    private async Task<EmployeeDto> GetEmployeeWithRelationships(Guid employeeId, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.EmployeeDepartments.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Department)
            .Include(e => e.TeamMemberships.Where(tm => !tm.IsDeleted))
                .ThenInclude(tm => tm.Team)
            .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, cancellationToken);

        return employee!.ToEmployeeDto();
    }
}