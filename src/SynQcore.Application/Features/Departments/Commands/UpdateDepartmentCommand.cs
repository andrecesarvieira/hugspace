using MediatR;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Commands;

/// <summary>
/// Command para atualizar departamento existente.
/// Permite reorganização hierárquica e modificação de propriedades.
/// </summary>
/// <param name="Id">ID do departamento a ser atualizado.</param>
/// <param name="Request">Novos dados do departamento.</param>
public record UpdateDepartmentCommand(Guid Id, UpdateDepartmentRequest Request) : IRequest<DepartmentDto>;
