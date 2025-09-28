using MediatR;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Queries;

/// <summary>
/// Query para obter detalhes de um funcionário específico por ID.
/// Retorna informações completas do funcionario.
/// </summary>
/// <param name="Id">Identificador único do funcionario.</param>
public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDto>;
