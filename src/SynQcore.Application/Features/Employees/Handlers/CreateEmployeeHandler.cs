using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.Commands;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Domain.Entities.Relationships;

namespace SynQcore.Application.Features.Employees.Handlers;

/// <summary>
/// Handler para processamento de criação de novos funcionários.
/// Gerencia validações, relacionamentos organizacionais e hierarquia.
/// </summary>
public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<CreateEmployeeHandler> _logger;

    private static readonly Action<ILogger, Guid, string, Exception?> LogEmployeeCreated =
        LoggerMessage.Define<Guid, string>(LogLevel.Information, new EventId(1, "EmployeeCreated"),
            "Employee created successfully: {EmployeeId} - {EmployeeName}");

    /// <summary>
    /// Inicializa nova instância do handler de criação de funcionários.
    /// </summary>
    /// <param name="context">Contexto de acesso a dados.</param>
    /// <param name="logger">Logger para rastreamento de operações.</param>
    public CreateEmployeeHandler(ISynQcoreDbContext context, ILogger<CreateEmployeeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Processa comando de criação de funcionário com validações e relacionamentos.
    /// </summary>
    /// <param name="request">Command contendo dados do funcionário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>DTO do funcionário criado com relacionamentos.</returns>
    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Validar se email já existe
        var existingEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == request.Request.Email && !e.IsDeleted, cancellationToken);

        if (existingEmployee != null)
            throw new ConflictException($"Employee with email {request.Request.Email} already exists");

        // Validar se manager existe (se especificado)
        if (request.Request.ManagerId.HasValue)
        {
            var manager = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Request.ManagerId.Value && !e.IsDeleted, cancellationToken);

            if (manager == null)
                throw new NotFoundException($"Manager with ID {request.Request.ManagerId.Value} not found");
        }

        // Criar employee
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            Email = request.Request.Email,
            Phone = request.Request.Phone,
            HireDate = request.Request.HireDate,
            ManagerId = request.Request.ManagerId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);

        // Adicionar relacionamentos com departamentos
        foreach (var deptId in request.Request.DepartmentIds)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == deptId && !d.IsDeleted, cancellationToken);

            if (department != null)
            {
                _context.EmployeeDepartments.Add(new EmployeeDepartment
                {
                    EmployeeId = employee.Id,
                    DepartmentId = deptId,
                    StartDate = DateTime.UtcNow
                });
            }
        }

        // Adicionar relacionamentos com teams
        foreach (var teamId in request.Request.TeamIds)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(t => t.Id == teamId && !t.IsDeleted, cancellationToken);

            if (team != null)
            {
                _context.TeamMemberships.Add(new TeamMembership
                {
                    EmployeeId = employee.Id,
                    TeamId = teamId,
                    JoinedAt = DateTime.UtcNow,
                    Role = Domain.Entities.Relationships.TeamRole.Member
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        LogEmployeeCreated(_logger, employee.Id, $"{employee.FirstName} {employee.LastName}", null);

        return await GetEmployeeWithRelationships(employee.Id, cancellationToken);
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

        if (employee == null)
            throw new NotFoundException($"Employee with ID {employeeId} not found");

        return employee.ToEmployeeDto();
    }
}
