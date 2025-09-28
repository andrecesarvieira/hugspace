using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.Commands;

namespace SynQcore.Application.Features.Employees.Handlers;

/// <summary>
/// Handler para exclusão lógica de funcionários do sistema.
/// Verifica dependências hierárquicas e remove relacionamentos organizacionais.
/// </summary>
public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<DeleteEmployeeHandler> _logger;

    private static readonly Action<ILogger, Guid, string, Exception?> LogEmployeeDeleted =
        LoggerMessage.Define<Guid, string>(LogLevel.Information, new EventId(1, "EmployeeDeleted"),
            "Employee deleted successfully: {EmployeeId} - {EmployeeName}");

    /// <summary>
    /// Inicializa nova instância do handler de exclusão de funcionários.
    /// </summary>
    /// <param name="context">Contexto de acesso a dados.</param>
    /// <param name="logger">Logger para rastreamento de operações.</param>
    public DeleteEmployeeHandler(ISynQcoreDbContext context, ILogger<DeleteEmployeeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Processa exclusão lógica de funcionário verificando dependências.
    /// </summary>
    /// <param name="request">Command contendo ID do funcionário a excluir.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

        if (employee == null)
            throw new NotFoundException($"Employee with ID {request.Id} not found");

        // Verificar se o employee não é manager de outros funcionários
        var hasSubordinates = await _context.Employees
            .AnyAsync(e => e.ManagerId == request.Id && !e.IsDeleted, cancellationToken);

        if (hasSubordinates)
            throw new ConflictException("Cannot delete employee who manages other employees. Please reassign subordinates first.");

        // Soft delete do employee
        employee.MarkAsDeleted();

        // Soft delete dos relacionamentos
        var employeeDepartments = await _context.EmployeeDepartments
            .Where(ed => ed.EmployeeId == request.Id && !ed.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var ed in employeeDepartments)
        {
            ed.MarkAsDeleted();
        }

        var teamMemberships = await _context.TeamMemberships
            .Where(tm => tm.EmployeeId == request.Id && !tm.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var tm in teamMemberships)
        {
            tm.MarkAsDeleted();
        }

        await _context.SaveChangesAsync(cancellationToken);

        LogEmployeeDeleted(_logger, request.Id, $"{employee.FirstName} {employee.LastName}", null);
    }
}
