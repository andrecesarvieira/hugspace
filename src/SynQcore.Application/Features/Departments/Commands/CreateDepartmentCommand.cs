using MediatR;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Commands;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : IRequest<DepartmentDto>;