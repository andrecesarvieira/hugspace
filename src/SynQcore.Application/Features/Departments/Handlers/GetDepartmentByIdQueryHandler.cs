using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Features.Departments.Queries;

namespace SynQcore.Application.Features.Departments.Handlers;

/// <summary>
/// Handler para buscar departamento por ID
/// </summary>
public partial class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetDepartmentByIdQueryHandler> _logger;

    public GetDepartmentByIdQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetDepartmentByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Buscando departamento por ID: {DepartmentId}")]
    private static partial void LogSearchingDepartment(ILogger logger, Guid departmentId, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Departamento encontrado: {DepartmentId} - {DepartmentName}")]
    private static partial void LogDepartmentFound(ILogger logger, Guid departmentId, string departmentName, Exception? exception);

    [LoggerMessage(LogLevel.Warning, "Departamento não encontrado: {DepartmentId}")]
    private static partial void LogDepartmentNotFound(ILogger logger, Guid departmentId, Exception? exception);

    public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        LogSearchingDepartment(_logger, request.Id, null);

        var department = await _context.Departments
            .Include(d => d.ParentDepartment)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
        {
            LogDepartmentNotFound(_logger, request.Id, null);
            throw new ArgumentException($"Departamento com ID {request.Id} não foi encontrado.");
        }

        LogDepartmentFound(_logger, department.Id, department.Name, null);

        // Mapear para DTO
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
