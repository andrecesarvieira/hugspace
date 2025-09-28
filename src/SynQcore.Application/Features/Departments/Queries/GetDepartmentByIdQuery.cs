using MediatR;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Queries;

/// <summary>
/// Query para obter detalhes de um departamento específico por ID.
/// Retorna informações completas do departamento.
/// </summary>
/// <param name="Id">Identificador único do departamento.</param>
public record GetDepartmentByIdQuery(Guid Id) : IRequest<DepartmentDto>;
