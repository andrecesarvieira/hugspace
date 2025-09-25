using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Features.Departments.Queries;

namespace SynQcore.Application.Features.Departments.Handlers;

public class GetDepartmentByIdHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentByIdHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DepartmentDto?> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .Include(d => d.ParentDepartment)
            .Include(d => d.SubDepartments.Where(c => !c.IsDeleted))
            .Include(d => d.Employees.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Employee)
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        return department != null ? _mapper.Map<DepartmentDto>(department) : null;
    }
}