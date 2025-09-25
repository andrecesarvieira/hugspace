using MediatR;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Commands;

public record UpdateEmployeeCommand(Guid Id, UpdateEmployeeRequest Request) : IRequest<EmployeeDto>;