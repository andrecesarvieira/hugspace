using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.Commands;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Handlers;

/// <summary>
/// Handler para atualizar departamentos existentes
/// </summary>
public partial class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UpdateDepartmentCommandHandler> _logger;

    public UpdateDepartmentCommandHandler(
        ISynQcoreDbContext context,
        ILogger<UpdateDepartmentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Atualizando departamento: {DepartmentId} - Novo nome: {DepartmentName}")]
    private static partial void LogUpdatingDepartment(ILogger logger, Guid departmentId, string departmentName, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Departamento atualizado com sucesso: {DepartmentId} - {DepartmentName}")]
    private static partial void LogDepartmentUpdated(ILogger logger, Guid departmentId, string departmentName, Exception? exception);

    [LoggerMessage(LogLevel.Warning, "Departamento não encontrado para atualização: {DepartmentId}")]
    private static partial void LogDepartmentNotFound(ILogger logger, Guid departmentId, Exception? exception);

    public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        LogUpdatingDepartment(_logger, request.Id, request.Request.Name, null);

        var department = await _context.Departments
            .Include(d => d.ParentDepartment)
            .Include(d => d.Manager)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
        {
            LogDepartmentNotFound(_logger, request.Id, null);
            throw new ArgumentException($"Departamento com ID {request.Id} não encontrado.");
        }

        // Verificar se código já existe em outro departamento
        if (!string.IsNullOrWhiteSpace(request.Request.Code) &&
            !string.Equals(request.Request.Code.Trim().ToUpperInvariant(), department.Code, StringComparison.OrdinalIgnoreCase))
        {
            var existingDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => string.Equals(d.Code, request.Request.Code.Trim().ToUpperInvariant(), StringComparison.OrdinalIgnoreCase) && d.Id != request.Id, cancellationToken);

            if (existingDepartment != null)
            {
                throw new InvalidOperationException($"Já existe um departamento com o código '{request.Request.Code}'.");
            }
        }

        // Verificar se departamento pai existe (se especificado e diferente)
        if (request.Request.ParentId.HasValue &&
            request.Request.ParentId.Value != department.ParentDepartmentId)
        {
            // Evitar referência circular
            if (request.Request.ParentId.Value == department.Id)
            {
                throw new InvalidOperationException("Um departamento não pode ser pai de si mesmo.");
            }

            var parentExists = await _context.Departments
                .AnyAsync(d => d.Id == request.Request.ParentId.Value && d.IsActive, cancellationToken);

            if (!parentExists)
            {
                throw new ArgumentException($"Departamento pai com ID {request.Request.ParentId.Value} não encontrado ou inativo.");
            }
        }

        // Atualizar propriedades
        department.Name = request.Request.Name.Trim();
        if (!string.IsNullOrWhiteSpace(request.Request.Code))
        {
            department.Code = request.Request.Code.Trim().ToUpperInvariant();
        }
        department.Description = request.Request.Description?.Trim();
        department.ParentDepartmentId = request.Request.ParentId;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        LogDepartmentUpdated(_logger, department.Id, department.Name, null);

        // Retornar DTO atualizado
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            Description = department.Description,
            IsActive = department.IsActive,
            ParentId = department.ParentDepartmentId,
            ParentName = department.ParentDepartment?.Name,
            CreatedAt = department.CreatedAt,
            UpdatedAt = department.UpdatedAt
        };
    }
}
