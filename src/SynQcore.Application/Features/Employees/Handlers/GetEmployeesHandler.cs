using System.Globalization;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;

namespace SynQcore.Application.Features.Employees.Handlers;

public class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeesHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
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
            var searchTerm = request.Request.SearchTerm.ToLower(CultureInfo.InvariantCulture);
            query = query.Where(e => 
                e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
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

        var employeeDtos = _mapper.Map<List<EmployeeDto>>(employees);

        return new PagedResult<EmployeeDto>
        {
            Items = employeeDtos,
            TotalCount = totalCount,
            Page = request.Request.Page,
            PageSize = request.Request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.Request.PageSize)
        };
    }
}