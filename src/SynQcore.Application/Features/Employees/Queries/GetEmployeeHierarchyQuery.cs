using MediatR;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Queries;

/// <summary>
/// Query para obter hierarquia organizacional de um funcionário.
/// Inclui gestor, subordinados e pares na estrutura organizacional.
/// </summary>
/// <param name="EmployeeId">ID do funcionario para obter hierarquia.</param>
public record GetEmployeeHierarchyQuery(Guid EmployeeId) : IRequest<EmployeeHierarchyDto>;
