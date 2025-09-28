using MediatR;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Commands;

public record CreateEmployeeCommand(CreateEmployeeRequest Request) : IRequest<EmployeeDto>;
