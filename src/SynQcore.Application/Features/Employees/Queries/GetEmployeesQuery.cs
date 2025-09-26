using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Queries;

public record GetEmployeesQuery(EmployeeSearchRequest Request) : IRequest<PagedResult<EmployeeDto>>;