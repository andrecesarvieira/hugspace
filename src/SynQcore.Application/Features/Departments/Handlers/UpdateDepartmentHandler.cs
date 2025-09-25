using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.Commands;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Handlers;

public class UpdateDepartmentHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public UpdateDepartmentHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .Include(d => d.ParentDepartment)
            .Include(d => d.SubDepartments.Where(sd => !sd.IsDeleted))
            .Include(d => d.Employees.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Employee)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
            throw new ArgumentException($"Department with ID '{request.Id}' not found.");

        // Verificar se o novo código já existe (se foi alterado)
        if (department.Code != request.Request.Code)
        {
            var existingDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Code == request.Request.Code && d.Id != request.Id, cancellationToken);

            if (existingDepartment != null)
                throw new InvalidOperationException($"Department with code '{request.Request.Code}' already exists.");
        }

        // Verificar se o novo departamento pai existe (se especificado)
        if (request.Request.ParentId.HasValue && request.Request.ParentId != department.ParentDepartmentId)
        {
            // Verificar se não está tentando ser pai de si mesmo
            if (request.Request.ParentId == request.Id)
                throw new InvalidOperationException("A department cannot be its own parent.");

            // Verificar se o novo pai existe
            var parentDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.Request.ParentId.Value, cancellationToken);

            if (parentDepartment == null)
                throw new ArgumentException($"Parent department with ID '{request.Request.ParentId}' not found.");

            // Verificar se não criaria uma referência circular
            if (await IsCircularReference(request.Id, request.Request.ParentId.Value, cancellationToken))
                throw new InvalidOperationException("The specified parent would create a circular reference.");
        }

        // Atualizar propriedades
        department.Name = request.Request.Name;
        department.Code = request.Request.Code;
        department.Description = request.Request.Description;
        department.ParentDepartmentId = request.Request.ParentId;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DepartmentDto>(department);
    }

    private async Task<bool> IsCircularReference(Guid departmentId, Guid newParentId, CancellationToken cancellationToken)
    {
        var currentParentId = newParentId;

        while (currentParentId != Guid.Empty)
        {
            if (currentParentId == departmentId)
                return true;

            var parent = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == currentParentId, cancellationToken);

            if (parent?.ParentDepartmentId == null)
                break;

            currentParentId = parent.ParentDepartmentId.Value;
        }

        return false;
    }
}