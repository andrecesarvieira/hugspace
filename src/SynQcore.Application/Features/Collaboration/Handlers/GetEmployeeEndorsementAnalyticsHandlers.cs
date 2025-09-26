using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Helpers;
using SynQcore.Application.Features.Collaboration.Queries;

namespace SynQcore.Application.Features.Collaboration.Handlers;

/// <summary>
/// Handler para obter endorsements dados por um funcionário específico
/// </summary>
public partial class GetEmployeeEndorsementsGivenQueryHandler : IRequestHandler<GetEmployeeEndorsementsGivenQuery, PagedResult<EndorsementDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetEmployeeEndorsementsGivenQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3081, Level = LogLevel.Information,
        Message = "Buscando endorsements dados por funcionário: {EmployeeId}")]
    private static partial void LogSearchingGivenEndorsements(ILogger logger, Guid employeeId);

    [LoggerMessage(EventId = 3082, Level = LogLevel.Information,
        Message = "Encontrados {Count} endorsements dados por {EmployeeId}")]
    private static partial void LogGivenEndorsementsFound(ILogger logger, int count, Guid employeeId);

    [LoggerMessage(EventId = 3083, Level = LogLevel.Warning,
        Message = "Funcionário não encontrado: {EmployeeId}")]
    private static partial void LogEmployeeNotFound(ILogger logger, Guid employeeId);

    [LoggerMessage(EventId = 3084, Level = LogLevel.Error,
        Message = "Erro ao buscar endorsements dados por {EmployeeId}")]
    private static partial void LogGivenEndorsementsError(ILogger logger, Guid employeeId, Exception ex);

    public GetEmployeeEndorsementsGivenQueryHandler(
        ISynQcoreDbContext context, 
        IMapper mapper, 
        ILogger<GetEmployeeEndorsementsGivenQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<EndorsementDto>> Handle(GetEmployeeEndorsementsGivenQuery request, CancellationToken cancellationToken)
    {
        LogSearchingGivenEndorsements(_logger, request.EmployeeId);

        try
        {
            // Verificar se o funcionário existe
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);
            if (!employeeExists)
            {
                LogEmployeeNotFound(_logger, request.EmployeeId);
                throw new ArgumentException($"Funcionário com ID {request.EmployeeId} não encontrado.");
            }

            // Query base para endorsements dados pelo funcionário
            var query = _context.Endorsements
                .Where(e => e.EndorserId == request.EmployeeId)
                .Include(e => e.Endorser)
                .Include(e => e.Post)
                .Include(e => e.Comment)
                .AsQueryable();

            // Aplicar filtros
            if (request.FilterByType.HasValue)
            {
                query = query.Where(e => e.Type == request.FilterByType.Value);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(e => e.EndorsedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(e => e.EndorsedAt <= request.EndDate.Value);
            }

            // Contagem total
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar paginação
            var endorsements = await query
                .OrderByDescending(e => e.EndorsedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Mapear para DTOs
            var endorsementDtos = endorsements.Select(e =>
            {
                var dto = _mapper.Map<EndorsementDto>(e);
                var typeInfo = EndorsementTypeHelper.GetTypeInfo(dto.Type);
                dto.TypeDisplayName = typeInfo.DisplayName;
                dto.TypeIcon = typeInfo.Icon;
                return dto;
            }).ToList();

            LogGivenEndorsementsFound(_logger, endorsementDtos.Count, request.EmployeeId);

            return new PagedResult<EndorsementDto>
            {
                Items = endorsementDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            LogGivenEndorsementsError(_logger, request.EmployeeId, ex);
            throw;
        }
    }
}

/// <summary>
/// Handler para obter endorsements recebidos por um funcionário específico
/// </summary>
public partial class GetEmployeeEndorsementsReceivedQueryHandler : IRequestHandler<GetEmployeeEndorsementsReceivedQuery, PagedResult<EndorsementDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetEmployeeEndorsementsReceivedQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3091, Level = LogLevel.Information,
        Message = "Buscando endorsements recebidos por funcionário: {EmployeeId}")]
    private static partial void LogSearchingReceivedEndorsements(ILogger logger, Guid employeeId);

    [LoggerMessage(EventId = 3092, Level = LogLevel.Information,
        Message = "Encontrados {Count} endorsements recebidos por {EmployeeId}")]
    private static partial void LogReceivedEndorsementsFound(ILogger logger, int count, Guid employeeId);

    [LoggerMessage(EventId = 3093, Level = LogLevel.Warning,
        Message = "Funcionário não encontrado para endorsements recebidos: {EmployeeId}")]
    private static partial void LogEmployeeNotFoundReceived(ILogger logger, Guid employeeId);

    [LoggerMessage(EventId = 3094, Level = LogLevel.Error,
        Message = "Erro ao buscar endorsements recebidos por {EmployeeId}")]
    private static partial void LogReceivedEndorsementsError(ILogger logger, Guid employeeId, Exception ex);

    public GetEmployeeEndorsementsReceivedQueryHandler(
        ISynQcoreDbContext context, 
        IMapper mapper, 
        ILogger<GetEmployeeEndorsementsReceivedQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<EndorsementDto>> Handle(GetEmployeeEndorsementsReceivedQuery request, CancellationToken cancellationToken)
    {
        LogSearchingReceivedEndorsements(_logger, request.EmployeeId);

        try
        {
            // Verificar se o funcionário existe
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);
            if (!employeeExists)
            {
                LogEmployeeNotFoundReceived(_logger, request.EmployeeId);
                throw new ArgumentException($"Funcionário com ID {request.EmployeeId} não encontrado.");
            }

            // Query para endorsements recebidos (em posts e comments do funcionário)
            var postEndorsementsQuery = _context.Endorsements
                .Where(e => e.Post != null && e.Post.AuthorId == request.EmployeeId)
                .Include(e => e.Endorser)
                .Include(e => e.Post)
                .AsQueryable();

            var commentEndorsementsQuery = _context.Endorsements
                .Where(e => e.Comment != null && e.Comment.AuthorId == request.EmployeeId)
                .Include(e => e.Endorser)
                .Include(e => e.Comment)
                .AsQueryable();

            // Aplicar filtros em ambas as queries
            if (request.FilterByType.HasValue)
            {
                postEndorsementsQuery = postEndorsementsQuery.Where(e => e.Type == request.FilterByType.Value);
                commentEndorsementsQuery = commentEndorsementsQuery.Where(e => e.Type == request.FilterByType.Value);
            }

            if (request.StartDate.HasValue)
            {
                postEndorsementsQuery = postEndorsementsQuery.Where(e => e.EndorsedAt >= request.StartDate.Value);
                commentEndorsementsQuery = commentEndorsementsQuery.Where(e => e.EndorsedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                postEndorsementsQuery = postEndorsementsQuery.Where(e => e.EndorsedAt <= request.EndDate.Value);
                commentEndorsementsQuery = commentEndorsementsQuery.Where(e => e.EndorsedAt <= request.EndDate.Value);
            }

            // Combinar resultados
            var postEndorsements = await postEndorsementsQuery.ToListAsync(cancellationToken);
            var commentEndorsements = await commentEndorsementsQuery.ToListAsync(cancellationToken);

            var allEndorsements = postEndorsements.Concat(commentEndorsements)
                .OrderByDescending(e => e.EndorsedAt)
                .ToList();

            var totalCount = allEndorsements.Count;

            // Aplicar paginação
            var pagedEndorsements = allEndorsements
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Mapear para DTOs
            var endorsementDtos = pagedEndorsements.Select(e =>
            {
                var dto = _mapper.Map<EndorsementDto>(e);
                var typeInfo = EndorsementTypeHelper.GetTypeInfo(dto.Type);
                dto.TypeDisplayName = typeInfo.DisplayName;
                dto.TypeIcon = typeInfo.Icon;
                return dto;
            }).ToList();

            LogReceivedEndorsementsFound(_logger, endorsementDtos.Count, request.EmployeeId);

            return new PagedResult<EndorsementDto>
            {
                Items = endorsementDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            LogReceivedEndorsementsError(_logger, request.EmployeeId, ex);
            throw;
        }
    }
}