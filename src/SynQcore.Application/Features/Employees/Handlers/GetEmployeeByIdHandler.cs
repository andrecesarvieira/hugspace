using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;

namespace SynQcore.Application.Features.Employees.Handlers;

public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeByIdHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Manager)
            .Include(e => e.EmployeeDepartments.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Department)
            .Include(e => e.TeamMemberships.Where(tm => !tm.IsDeleted))
                .ThenInclude(tm => tm.Team)
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

        if (employee == null)
            throw new NotFoundException($"Employee with ID {request.Id} not found");

        return _mapper.Map<EmployeeDto>(employee);
    }
}