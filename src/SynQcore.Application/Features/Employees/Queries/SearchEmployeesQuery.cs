using MediatR;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Queries;

public record SearchEmployeesQuery(string SearchTerm) : IRequest<List<EmployeeDto>>;
