using System.Globalization;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;

namespace SynQcore.Application.Features.Employees.Handlers;

public class SearchEmployeesHandler : IRequestHandler<SearchEmployeesQuery, List<EmployeeDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public SearchEmployeesHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EmployeeDto>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return new List<EmployeeDto>();

        var searchTerm = request.SearchTerm.ToLower(CultureInfo.InvariantCulture);

        var employees = await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.EmployeeDepartments.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Department)
            .Include(e => e.TeamMemberships.Where(tm => !tm.IsDeleted))
                .ThenInclude(tm => tm.Team)
            .Where(e => !e.IsDeleted && e.IsActive &&
                       (e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Take(20) // Limitar resultados para performance
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<EmployeeDto>>(employees);
    }
}