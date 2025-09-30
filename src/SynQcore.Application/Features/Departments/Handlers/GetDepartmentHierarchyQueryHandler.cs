using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Features.Departments.Queries;

namespace SynQcore.Application.Features.Departments.Handlers;

/// <summary>
/// Handler para buscar hierarquia completa de um departamento
/// </summary>
public partial class GetDepartmentHierarchyQueryHandler : IRequestHandler<GetDepartmentHierarchyQuery, DepartmentHierarchyDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetDepartmentHierarchyQueryHandler> _logger;

    public GetDepartmentHierarchyQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetDepartmentHierarchyQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Buscando hierarquia do departamento: {DepartmentId}")]
    private static partial void LogSearchingHierarchy(ILogger logger, Guid departmentId, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Hierarquia encontrada - Departamento: {DepartmentName}, Subdepartamentos: {SubCount}")]
    private static partial void LogHierarchyFound(ILogger logger, string departmentName, int subCount, Exception? exception);

    public async Task<DepartmentHierarchyDto> Handle(GetDepartmentHierarchyQuery request, CancellationToken cancellationToken)
    {
        LogSearchingHierarchy(_logger, request.DepartmentId, null);

        var department = await _context.Departments
            .Include(d => d.ParentDepartment)
            .Include(d => d.Manager)
            .Include(d => d.SubDepartments)
                .ThenInclude(sd => sd.Manager)
            .Include(d => d.SubDepartments)
                .ThenInclude(sd => sd.SubDepartments)
                    .ThenInclude(ssd => ssd.Manager)
            .FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

        if (department == null)
        {
            throw new ArgumentException($"Departamento com ID {request.DepartmentId} não encontrado.");
        }

        // Buscar funcionários do departamento
        var employees = await _context.EmployeeDepartments
            .Where(ed => ed.DepartmentId == department.Id && ed.Employee.IsActive)
            .Select(ed => new EmployeeSummaryDto
            {
                Id = ed.Employee.Id,
                FirstName = ed.Employee.FirstName,
                LastName = ed.Employee.LastName,
                Email = ed.Employee.Email,
                AvatarUrl = ed.Employee.ProfilePhotoUrl,
                IsActive = ed.Employee.IsActive,
                HireDate = ed.Employee.HireDate
            })
            .ToListAsync(cancellationToken);

        // Construir hierarquia
        var hierarchy = new DepartmentHierarchyDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            IsActive = department.IsActive,
            Level = 0, // Calcularemos depois
            HierarchyPath = department.Name, // Simplificado
            DirectEmployeesCount = employees.Count,
            TotalEmployeesInHierarchy = employees.Count, // Simplificado
            DirectEmployees = employees,
            Parent = department.ParentDepartment != null ? new DepartmentHierarchyDto
            {
                Id = department.ParentDepartment.Id,
                Name = department.ParentDepartment.Name,
                Description = department.ParentDepartment.Description,
                IsActive = department.ParentDepartment.IsActive,
                Level = 0,
                HierarchyPath = department.ParentDepartment.Name,
                DirectEmployeesCount = 0,
                TotalEmployeesInHierarchy = 0,
                DirectEmployees = new List<EmployeeSummaryDto>(),
                Children = new List<DepartmentHierarchyDto>()
            } : null,
            Children = department.SubDepartments
                .Where(sd => sd.IsActive)
                .Select(sd => new DepartmentHierarchyDto
                {
                    Id = sd.Id,
                    Name = sd.Name,
                    Description = sd.Description,
                    IsActive = sd.IsActive,
                    Level = 1,
                    HierarchyPath = $"{department.Name} / {sd.Name}",
                    DirectEmployeesCount = 0,
                    TotalEmployeesInHierarchy = 0,
                    DirectEmployees = new List<EmployeeSummaryDto>(),
                    Children = new List<DepartmentHierarchyDto>()
                }).ToList()
        };

        LogHierarchyFound(_logger, department.Name, hierarchy.Children.Count, null);

        return hierarchy;
    }
}
