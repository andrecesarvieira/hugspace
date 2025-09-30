using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Features.Departments.Commands;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Features.Departments.Handlers;

/// <summary>
/// Handler para criar novos departamentos
/// </summary>
public partial class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<CreateDepartmentCommandHandler> _logger;

    public CreateDepartmentCommandHandler(
        ISynQcoreDbContext context,
        ILogger<CreateDepartmentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Criando departamento: {DepartmentName} - Código: {DepartmentCode}")]
    private static partial void LogCreatingDepartment(ILogger logger, string departmentName, string departmentCode, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Departamento criado com sucesso: {DepartmentId} - {DepartmentName}")]
    private static partial void LogDepartmentCreated(ILogger logger, Guid departmentId, string departmentName, Exception? exception);

    [LoggerMessage(LogLevel.Warning, "Tentativa de criar departamento com código duplicado: {DepartmentCode}")]
    private static partial void LogDuplicateCode(ILogger logger, string departmentCode, Exception? exception);

    public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        LogCreatingDepartment(_logger, request.Request.Name, request.Request.Code, null);

        // Verificar se código já existe
        var existingDepartment = await _context.Departments
            .FirstOrDefaultAsync(d => d.Code == request.Request.Code, cancellationToken);

        if (existingDepartment != null)
        {
            LogDuplicateCode(_logger, request.Request.Code, null);
            throw new InvalidOperationException($"Já existe um departamento com o código '{request.Request.Code}'.");
        }

        // Verificar se departamento pai existe (se especificado)
        if (request.Request.ParentId.HasValue)
        {
            var parentExists = await _context.Departments
                .AnyAsync(d => d.Id == request.Request.ParentId.Value && d.IsActive, cancellationToken);

            if (!parentExists)
            {
                throw new ArgumentException($"Departamento pai com ID {request.Request.ParentId.Value} não encontrado ou inativo.");
            }
        }

        // Criar o departamento
        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = request.Request.Name.Trim(),
            Code = request.Request.Code.Trim().ToUpperInvariant(),
            Description = request.Request.Description?.Trim(),
            ParentDepartmentId = request.Request.ParentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);

        LogDepartmentCreated(_logger, department.Id, department.Name, null);

        // Retornar DTO
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
