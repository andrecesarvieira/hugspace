using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;

namespace SynQcore.Application.Features.Employees.Handlers;

public class SearchEmployeesHandler : IRequestHandler<SearchEmployeesQuery, List<EmployeeDto>>
{
    private readonly ISynQcoreDbContext _context;

    public SearchEmployeesHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeDto>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return new List<EmployeeDto>();

        var searchTerm = request.SearchTerm.ToLower(CultureInfo.InvariantCulture);

        var employees = await _context.Employees
            .Where(e => e.FirstName.Contains(request.SearchTerm) || 
                       e.LastName.Contains(request.SearchTerm) || 
                       e.Email.Contains(request.SearchTerm) ||
                       e.JobTitle.Contains(request.SearchTerm))
            .ToListAsync(cancellationToken);

        return employees.ToEmployeeDtos();
    }
}