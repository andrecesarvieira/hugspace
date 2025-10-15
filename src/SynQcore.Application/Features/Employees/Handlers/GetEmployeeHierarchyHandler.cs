using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;

namespace SynQcore.Application.Features.Employees.Handlers;

public class GetEmployeeHierarchyHandler : IRequestHandler<GetEmployeeHierarchyQuery, EmployeeHierarchyDto>
{
    private readonly ISynQcoreDbContext _context;

    public GetEmployeeHierarchyHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeHierarchyDto> Handle(GetEmployeeHierarchyQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.EmployeeDepartments.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Department)
            .Include(e => e.TeamMemberships.Where(tm => !tm.IsDeleted))
                .ThenInclude(tm => tm.Team)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && !e.IsDeleted, cancellationToken);

        if (employee == null)
            throw new NotFoundException($"Employee with ID {request.EmployeeId} not found");

        // Buscar subordinados diretos
        var subordinates = await _context.Employees
            .Include(e => e.EmployeeDepartments.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Department)
            .Include(e => e.TeamMemberships.Where(tm => !tm.IsDeleted))
                .ThenInclude(tm => tm.Team)
            .Where(e => e.ManagerId == request.EmployeeId && !e.IsDeleted)
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToListAsync(cancellationToken);

        // Buscar colegas (mesmo manager)
        var peers = new List<Domain.Entities.Organization.Employee>();
        if (employee.ManagerId.HasValue)
        {
            peers = await _context.Employees
                .Include(e => e.EmployeeDepartments.Where(ed => !ed.IsDeleted))
                    .ThenInclude(ed => ed.Department)
                .Include(e => e.TeamMemberships.Where(tm => !tm.IsDeleted))
                    .ThenInclude(tm => tm.Team)
                .Where(e => e.ManagerId == employee.ManagerId.Value &&
                           e.Id != request.EmployeeId &&
                           !e.IsDeleted)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync(cancellationToken);
        }

        return new EmployeeHierarchyDto
        {
            Employee = employee.ToEmployeeDto(),
            Manager = employee.Manager?.ToEmployeeDto(),
            Subordinates = subordinates.ToEmployeeDtos(),
            Peers = peers.ToEmployeeDtos()
        };
    }
}
