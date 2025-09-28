using MediatR;

namespace SynQcore.Application.Features.Employees.Commands;

/// <summary>
/// Command para excluir funcionario da organização.
/// Realiza desativação lógica preservando histórico.
/// </summary>
/// <param name="Id">ID do funcionario a ser excluído.</param>
public record DeleteEmployeeCommand(Guid Id) : IRequest;
