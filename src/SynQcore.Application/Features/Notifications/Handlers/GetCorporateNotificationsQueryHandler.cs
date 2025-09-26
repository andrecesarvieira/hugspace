using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Notifications.Queries;

namespace SynQcore.Application.Features.Notifications.Handlers;

/// <summary>
/// Handler para busca de notificações corporativas
/// </summary>
public partial class GetCorporateNotificationsQueryHandler : IRequestHandler<GetCorporateNotificationsQuery, GetCorporateNotificationsResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetCorporateNotificationsQueryHandler> _logger;

    public GetCorporateNotificationsQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetCorporateNotificationsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetCorporateNotificationsResponse> Handle(GetCorporateNotificationsQuery request, CancellationToken cancellationToken)
    {
        LogSearchingCorporateNotifications(_logger, request.Page, request.PageSize);

        var query = _context.CorporateNotifications
            .Include(n => n.CreatedByEmployee)
            .Include(n => n.TargetDepartment)
            .Include(n => n.ApprovedByEmployee)
            .AsQueryable();

        // Aplicar filtros por strings (apenas filtro básico)
        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            query = query.Where(n => n.Type.ToString().Contains(request.Type));
        }

        if (!string.IsNullOrWhiteSpace(request.Priority))
        {
            query = query.Where(n => n.Priority.ToString().Contains(request.Priority));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(n => n.Status.ToString().Contains(request.Status));
        }

        if (request.TargetDepartmentId.HasValue)
        {
            query = query.Where(n => n.TargetDepartmentId == request.TargetDepartmentId.Value);
        }

        if (request.CreatedBy.HasValue)
        {
            query = query.Where(n => n.CreatedByEmployeeId == request.CreatedBy.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(n => n.CreatedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(n => n.CreatedAt <= request.DateTo.Value);
        }

        // Contar total de registros
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginação e ordenação
        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        LogCorporateNotificationsFound(_logger, notifications.Count, totalCount);

        return new GetCorporateNotificationsResponse
        {
            Notifications = notifications.ToCorporateNotificationDtos(),
            TotalCount = totalCount,
            CurrentPage = request.Page,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    [LoggerMessage(EventId = 5011, Level = LogLevel.Information,
        Message = "Buscando notificações corporativas - Página: {PageNumber}, Tamanho: {PageSize}")]
    private static partial void LogSearchingCorporateNotifications(ILogger logger, int pageNumber, int pageSize);

    [LoggerMessage(EventId = 5012, Level = LogLevel.Information,
        Message = "Encontradas {NotificationsCount} notificações corporativas de um total de {TotalCount}")]
    private static partial void LogCorporateNotificationsFound(ILogger logger, int notificationsCount, int totalCount);
}