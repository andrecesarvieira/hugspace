using MediatR;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Departments.Queries;

public record GetDepartmentsQuery(GetDepartmentsRequest Request) : IRequest<PagedResult<DTOs.DepartmentDto>>;