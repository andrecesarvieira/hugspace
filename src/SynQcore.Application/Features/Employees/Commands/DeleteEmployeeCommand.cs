using MediatR;

namespace SynQcore.Application.Features.Employees.Commands;

public record DeleteEmployeeCommand(Guid Id) : IRequest;
