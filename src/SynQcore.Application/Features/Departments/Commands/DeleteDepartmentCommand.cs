using MediatR;

namespace SynQcore.Application.Features.Departments.Commands;

/// <summary>
/// Command para excluir departamento da estrutura organizacional.
/// Verifica dependências e realiza desativação segura.
/// </summary>
/// <param name="Id">ID do departamento a ser excluído.</param>
public record DeleteDepartmentCommand(Guid Id) : IRequest<Unit>;
