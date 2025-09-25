using MediatR;

namespace SynQcore.Application.Features.Departments.Commands;

public record DeleteDepartmentCommand(Guid Id) : IRequest<Unit>;