using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.DocumentTemplates.DTOs;
using SynQcore.Application.Features.DocumentTemplates.Queries;

namespace SynQcore.Application.Features.DocumentTemplates.Handlers;

/// <summary>
/// Handler para obter template específico por ID
/// </summary>
public partial class GetTemplateByIdQueryHandler : IRequestHandler<GetTemplateByIdQuery, DocumentTemplateDetailDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetTemplateByIdQueryHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Buscando template por ID: {TemplateId}")]
    private static partial void LogBuscandoTemplatePorId(ILogger logger, Guid templateId, Exception? exception);

    public GetTemplateByIdQueryHandler(ISynQcoreDbContext context, ILogger<GetTemplateByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DocumentTemplateDetailDto?> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoTemplatePorId(_logger, request.TemplateId, null);

        var template = await _context.DocumentTemplates
            .Include(t => t.CreatedByEmployee)
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId && !t.IsDeleted, cancellationToken);

        return template?.ToDocumentTemplateDetailDto();
    }
}

/// <summary>
/// Handler para obter estatísticas de uso do template
/// </summary>
public partial class GetTemplateUsageStatsQueryHandler : IRequestHandler<GetTemplateUsageStatsQuery, TemplateUsageStatsDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetTemplateUsageStatsQueryHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Buscando estatísticas do template: {TemplateId}")]
    private static partial void LogBuscandoEstatisticasTemplate(ILogger logger, Guid templateId, Exception? exception);

    public GetTemplateUsageStatsQueryHandler(ISynQcoreDbContext context, ILogger<GetTemplateUsageStatsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TemplateUsageStatsDto?> Handle(GetTemplateUsageStatsQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoEstatisticasTemplate(_logger, request.TemplateId, null);

        var template = await _context.DocumentTemplates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId && !t.IsDeleted, cancellationToken);

        if (template == null) return null;

        // Simular estatísticas - em uma implementação real, viria de uma tabela de uso
        return new TemplateUsageStatsDto
        {
            TemplateId = template.Id,
            TotalUsages = new Random().Next(1, 100),
            LastUsedAt = DateTime.UtcNow.AddDays(-new Random().Next(1, 30)),
            UniqueUsers = new Random().Next(1, 20),
            UsagesByDepartment = new Dictionary<string, int>
            {
                ["TI"] = new Random().Next(1, 10),
                ["RH"] = new Random().Next(1, 15),
                ["Vendas"] = new Random().Next(1, 20)
            },
            UsagesByDate = new Dictionary<DateTime, int>
            {
                [DateTime.UtcNow.AddDays(-7)] = new Random().Next(1, 5),
                [DateTime.UtcNow.AddDays(-14)] = new Random().Next(1, 8),
                [DateTime.UtcNow.AddDays(-21)] = new Random().Next(1, 12)
            }
        };
    }
}

/// <summary>
/// Handler para obter templates por categoria
/// </summary>
public partial class GetTemplatesByCategoryQueryHandler : IRequestHandler<GetTemplatesByCategoryQuery, PagedResult<DocumentTemplateDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetTemplatesByCategoryQueryHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Buscando templates por categoria: {Category}")]
    private static partial void LogBuscandoTemplatesPorCategoria(ILogger logger, string category, Exception? exception);

    public GetTemplatesByCategoryQueryHandler(ISynQcoreDbContext context, ILogger<GetTemplatesByCategoryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<DocumentTemplateDto>> Handle(GetTemplatesByCategoryQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoTemplatesPorCategoria(_logger, request.Category, null);

        var query = _context.DocumentTemplates
            .Where(t => !t.IsDeleted && t.DefaultCategory.ToString().Contains(request.Category));

        return await query.OrderBy(t => t.Name)
            .ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}

/// <summary>
/// Handler para obter templates ativos
/// </summary>
public partial class GetActiveTemplatesQueryHandler : IRequestHandler<GetActiveTemplatesQuery, List<DocumentTemplateDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetActiveTemplatesQueryHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Buscando templates ativos")]
    private static partial void LogBuscandoTemplatesAtivos(ILogger logger, Exception? exception);

    public GetActiveTemplatesQueryHandler(ISynQcoreDbContext context, ILogger<GetActiveTemplatesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DocumentTemplateDto>> Handle(GetActiveTemplatesQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoTemplatesAtivos(_logger, null);

        var templates = await _context.DocumentTemplates
            .Where(t => !t.IsDeleted && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return templates.Select(t => t.ToDocumentTemplateDto()).ToList();
    }
}

/// <summary>
/// Handler para obter templates populares
/// </summary>
public partial class GetPopularTemplatesQueryHandler : IRequestHandler<GetPopularTemplatesQuery, List<DocumentTemplateDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetPopularTemplatesQueryHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Buscando templates populares - Limite: {Limit}")]
    private static partial void LogBuscandoTemplatesPopulares(ILogger logger, int limit, Exception? exception);

    public GetPopularTemplatesQueryHandler(ISynQcoreDbContext context, ILogger<GetPopularTemplatesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DocumentTemplateDto>> Handle(GetPopularTemplatesQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoTemplatesPopulares(_logger, request.Limit, null);

        var templates = await _context.DocumentTemplates
            .Where(t => !t.IsDeleted && t.IsActive)
            .OrderByDescending(t => t.UsageCount)
            .ThenBy(t => t.Name)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return templates.Select(t => t.ToDocumentTemplateDto()).ToList();
    }
}

/// <summary>
/// Handler para obter meus templates
/// </summary>
public partial class GetMyTemplatesQueryHandler : IRequestHandler<GetMyTemplatesQuery, PagedResult<DocumentTemplateDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetMyTemplatesQueryHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Buscando templates do usuário: {UserId}")]
    private static partial void LogBuscandoMeusTemplates(ILogger logger, Guid userId, Exception? exception);

    public GetMyTemplatesQueryHandler(ISynQcoreDbContext context, ILogger<GetMyTemplatesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<DocumentTemplateDto>> Handle(GetMyTemplatesQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoMeusTemplates(_logger, request.UserId, null);

        var query = _context.DocumentTemplates
            .Where(t => !t.IsDeleted && t.CreatedByEmployeeId == request.UserId);

        return await query.OrderByDescending(t => t.CreatedAt)
            .ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}

/// <summary>
/// Handler para obter templates do departamento
/// </summary>
public partial class GetDepartmentTemplatesQueryHandler : IRequestHandler<GetDepartmentTemplatesQuery, List<DocumentTemplateDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetDepartmentTemplatesQueryHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Buscando templates do departamento: {DepartmentId}")]
    private static partial void LogBuscandoTemplatesDepartamento(ILogger logger, Guid departmentId, Exception? exception);

    public GetDepartmentTemplatesQueryHandler(ISynQcoreDbContext context, ILogger<GetDepartmentTemplatesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DocumentTemplateDto>> Handle(GetDepartmentTemplatesQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoTemplatesDepartamento(_logger, request.DepartmentId, null);

        var templates = await _context.DocumentTemplates
            .Where(t => !t.IsDeleted && t.IsActive &&
                t.CreatedByEmployee != null &&
                t.CreatedByEmployee.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return templates.Select(t => t.ToDocumentTemplateDto()).ToList();
    }
}
