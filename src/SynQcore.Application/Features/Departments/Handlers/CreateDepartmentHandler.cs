using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.Commands;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Features.Departments.Handlers;

public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public CreateDepartmentHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        // Verificar se o código do departamento já existe
        var existingDepartment = await _context.Departments
            .FirstOrDefaultAsync(d => d.Code == request.Request.Code, cancellationToken);

        if (existingDepartment != null)
            throw new InvalidOperationException($"Department with code '{request.Request.Code}' already exists.");

        // Verificar se o departamento pai existe (se especificado)
        if (request.Request.ParentId.HasValue)
        {
            var parentDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.Request.ParentId.Value, cancellationToken);

            if (parentDepartment == null)
                throw new ArgumentException($"Parent department with ID '{request.Request.ParentId}' not found.");
        }

        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = request.Request.Name,
            Code = request.Request.Code,
            Description = request.Request.Description,
            ParentDepartmentId = request.Request.ParentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);

        // Recarregar com dados relacionados para mapeamento completo
        var createdDepartment = await _context.Departments
            .Include(d => d.ParentDepartment)
            .Include(d => d.SubDepartments.Where(sd => !sd.IsDeleted))
            .Include(d => d.Employees.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Employee)
            .FirstAsync(d => d.Id == department.Id, cancellationToken);

        return _mapper.Map<DepartmentDto>(createdDepartment);
    }
}