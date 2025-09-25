using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.Commands;

namespace SynQcore.Application.Features.Departments.Handlers;

public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentCommand, Unit>
{
    private readonly ISynQcoreDbContext _context;

    public DeleteDepartmentHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .Include(d => d.SubDepartments)
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
            throw new ArgumentException($"Department with ID '{request.Id}' not found.");

        // Verificar se há departamentos filhos
        if (department.SubDepartments.Any(sd => !sd.IsDeleted))
            throw new InvalidOperationException("Cannot delete a department that has child departments. Please reassign or delete child departments first.");

        // Verificar se há funcionários associados
        if (department.Employees.Any(ed => !ed.IsDeleted))
            throw new InvalidOperationException("Cannot delete a department that has active employees. Please reassign employees to other departments first.");

        // Soft delete
        department.IsDeleted = true;
        department.DeletedAt = DateTime.UtcNow;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}