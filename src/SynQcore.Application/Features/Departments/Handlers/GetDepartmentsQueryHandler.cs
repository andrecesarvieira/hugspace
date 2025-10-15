using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Features.Departments.Queries;

namespace SynQcore.Application.Features.Departments.Handlers;

/// <summary>
/// Handler para buscar departamentos com paginação e filtros
/// </summary>
public partial class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, PagedResult<DepartmentDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetDepartmentsQueryHandler> _logger;

    public GetDepartmentsQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetDepartmentsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Processando busca de departamentos - Página: {Page}, Tamanho: {PageSize}")]
    private static partial void LogProcessingDepartments(ILogger logger, int page, int pageSize, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Busca concluída - {Count} departamentos encontrados")]
    private static partial void LogDepartmentsFound(ILogger logger, int count, Exception? exception);

    public async Task<PagedResult<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        LogProcessingDepartments(_logger, request.Request.Page, request.Request.PageSize, null);

        var query = _context.Departments.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(request.Request.Name))
        {
            var searchTerm = request.Request.Name;
            query = query.Where(d => d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Request.Code))
        {
            var searchCode = request.Request.Code;
            query = query.Where(d => d.Code.Contains(searchCode, StringComparison.OrdinalIgnoreCase));
        }

        if (request.Request.ParentId.HasValue)
        {
            query = query.Where(d => d.ParentDepartmentId == request.Request.ParentId.Value);
        }

        if (request.Request.IsActive.HasValue)
        {
            query = query.Where(d => d.IsActive == request.Request.IsActive.Value);
        }

        // Incluir relacionamentos necessários
        query = query.Include(d => d.ParentDepartment)
                     .Include(d => d.Manager);

        // Aplicar ordenação
        var orderBy = "name"; // Valor padrão
        query = orderBy switch
        {
            "name" => query.OrderBy(d => d.Name),
            "code" => query.OrderBy(d => d.Code),
            "createdat" => query.OrderBy(d => d.CreatedAt),
            _ => query.OrderBy(d => d.Name)
        };

        var departments = await query
            .Skip((request.Request.Page - 1) * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await _context.Departments.CountAsync(cancellationToken);

        // Converter para DTOs
        var departmentDtos = departments.Select(d => new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            Code = d.Code,
            Description = d.Description,
            IsActive = d.IsActive,
            ParentId = d.ParentDepartmentId,
            ParentName = d.ParentDepartment?.Name,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        }).ToList();

        LogDepartmentsFound(_logger, departmentDtos.Count, null);

        return new PagedResult<DepartmentDto>
        {
            Items = departmentDtos,
            TotalCount = totalCount,
            Page = request.Request.Page,
            PageSize = request.Request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.Request.PageSize)
        };
    }
}
