using MediatR;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Commands;

/// <summary>
/// Command para criar novo departamento na estrutura organizacional.
/// Estabelece hierarquia e organização corporativa.
/// </summary>
/// <param name="Request">Dados do departamento a ser criado.</param>
public record CreateDepartmentCommand(CreateDepartmentRequest Request) : IRequest<DepartmentDto>;
