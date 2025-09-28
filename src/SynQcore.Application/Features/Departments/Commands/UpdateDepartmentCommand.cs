using MediatR;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Commands;

public record UpdateDepartmentCommand(Guid Id, UpdateDepartmentRequest Request) : IRequest<DepartmentDto>;
