using MediatR;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Queries;

public record GetDepartmentHierarchyQuery(Guid DepartmentId) : IRequest<DepartmentHierarchyDto>;
