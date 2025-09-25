using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Features.Departments.Queries;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Departments.Handlers;

public class GetDepartmentsHandler : IRequestHandler<GetDepartmentsQuery, PagedResult<DTOs.DepartmentDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentsHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<DTOs.DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Departments
            .Where(d => !d.IsDeleted)
            .AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(request.Request.Name))
        {
            query = query.Where(d => d.Name.Contains(request.Request.Name));
        }

        if (!string.IsNullOrWhiteSpace(request.Request.Code))
        {
            query = query.Where(d => d.Code.Contains(request.Request.Code));
        }

        if (request.Request.ParentId.HasValue)
        {
            query = query.Where(d => d.ParentDepartmentId == request.Request.ParentId);
        }

        if (request.Request.IsActive.HasValue)
        {
            query = query.Where(d => d.IsActive == request.Request.IsActive.Value);
        }

        // Incluir relacionamentos
        query = query
            .Include(d => d.ParentDepartment)
            .Include(d => d.SubDepartments.Where(c => !c.IsDeleted));

        if (request.Request.IncludeEmployees)
        {
            query = query.Include(d => d.Employees.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Employee);
        }

        // Contar total
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginação
        var departments = await query
            .OrderBy(d => d.Name)
            .Skip((request.Request.Page - 1) * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .ToListAsync(cancellationToken);

        var departmentDtos = _mapper.Map<List<DTOs.DepartmentDto>>(departments);

        return new PagedResult<DTOs.DepartmentDto>
        {
            Items = departmentDtos,
            TotalCount = totalCount,
            Page = request.Request.Page,
            PageSize = request.Request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.Request.PageSize)
        };
    }
}