using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Features.Departments.Queries;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Features.Departments.Handlers;

public class GetDepartmentHierarchyHandler : IRequestHandler<GetDepartmentHierarchyQuery, DepartmentHierarchyDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentHierarchyHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DepartmentHierarchyDto> Handle(GetDepartmentHierarchyQuery request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .Include(d => d.ParentDepartment)
            .Include(d => d.Employees.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Employee)
            .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && !d.IsDeleted, cancellationToken);

        if (department == null)
            throw new ArgumentException($"Department with ID '{request.DepartmentId}' not found.");

        // Buscar todos os departamentos filhos recursivamente
        var children = await GetChildrenRecursively(request.DepartmentId, cancellationToken);

        var hierarchyDto = _mapper.Map<DepartmentHierarchyDto>(department);
        hierarchyDto.Children = MapChildrenToHierarchy(children);

        return hierarchyDto;
    }

    private async Task<List<Department>> GetChildrenRecursively(Guid parentId, CancellationToken cancellationToken)
    {
        var directChildren = await _context.Departments
            .Include(d => d.Employees.Where(ed => !ed.IsDeleted))
                .ThenInclude(ed => ed.Employee)
            .Where(d => d.ParentDepartmentId == parentId && !d.IsDeleted)
            .ToListAsync(cancellationToken);

        var allChildren = new List<Department>(directChildren);

        foreach (var child in directChildren)
        {
            var grandChildren = await GetChildrenRecursively(child.Id, cancellationToken);
            allChildren.AddRange(grandChildren);
        }

        return allChildren;
    }

    private List<DepartmentHierarchyDto> MapChildrenToHierarchy(List<Department> allChildren)
    {
        var hierarchyDtos = new List<DepartmentHierarchyDto>();
        var departmentGroups = allChildren
            .Where(d => d.ParentDepartmentId.HasValue)
            .GroupBy(d => d.ParentDepartmentId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var department in allChildren)
        {
            var hierarchyDto = _mapper.Map<DepartmentHierarchyDto>(department);
            
            // Buscar filhos diretos
            if (departmentGroups.TryGetValue(department.Id, out var children))
            {
                hierarchyDto.Children = children.Select(c => _mapper.Map<DepartmentHierarchyDto>(c)).ToList();
            }
            else
            {
                hierarchyDto.Children = new List<DepartmentHierarchyDto>();
            }

            hierarchyDtos.Add(hierarchyDto);
        }

        return hierarchyDtos;
    }
}