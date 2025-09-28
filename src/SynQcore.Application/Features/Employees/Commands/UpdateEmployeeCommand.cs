using MediatR;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Commands;

/// <summary>
/// Command para atualizar dados de funcionario existente.
/// Permite modificação de informações pessoais e organizacionais.
/// </summary>
/// <param name="Id">ID do funcionario a ser atualizado.</param>
/// <param name="Request">Novos dados do funcionario.</param>
public record UpdateEmployeeCommand(Guid Id, UpdateEmployeeRequest Request) : IRequest<EmployeeDto>;
