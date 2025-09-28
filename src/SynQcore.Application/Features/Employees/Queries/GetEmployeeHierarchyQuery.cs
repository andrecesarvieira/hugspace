using MediatR;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Queries;

public record GetEmployeeHierarchyQuery(Guid EmployeeId) : IRequest<EmployeeHierarchyDto>;
