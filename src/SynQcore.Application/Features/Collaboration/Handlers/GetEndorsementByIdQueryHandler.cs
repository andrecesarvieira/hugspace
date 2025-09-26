using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Helpers;
using SynQcore.Application.Features.Collaboration.Queries;

namespace SynQcore.Application.Features.Collaboration.Handlers;

/// <summary>
/// Handler para buscar endorsement específico por ID com informações completas
/// </summary>
public partial class GetEndorsementByIdQueryHandler : IRequestHandler<GetEndorsementByIdQuery, EndorsementDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetEndorsementByIdQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3031, Level = LogLevel.Information,
        Message = "Buscando endorsement por ID: {EndorsementId}")]
    private static partial void LogSearchingEndorsement(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 3032, Level = LogLevel.Information,
        Message = "Endorsement encontrado: {EndorsementId} tipo {Type}")]
    private static partial void LogEndorsementFound(ILogger logger, Guid endorsementId, SynQcore.Domain.Entities.Communication.EndorsementType type);

    [LoggerMessage(EventId = 3033, Level = LogLevel.Warning,
        Message = "Endorsement não encontrado: {EndorsementId}")]
    private static partial void LogEndorsementNotFound(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 3034, Level = LogLevel.Error,
        Message = "Erro ao buscar endorsement: {EndorsementId}")]
    private static partial void LogEndorsementSearchError(ILogger logger, Guid endorsementId, Exception ex);

    public GetEndorsementByIdQueryHandler(
        ISynQcoreDbContext context, 
        IMapper mapper, 
        ILogger<GetEndorsementByIdQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EndorsementDto> Handle(GetEndorsementByIdQuery request, CancellationToken cancellationToken)
    {
        LogSearchingEndorsement(_logger, request.Id);

        try
        {
            // Buscar endorsement com todas as informações relacionadas
            var endorsement = await _context.Endorsements
                .Include(e => e.Endorser)
                .Include(e => e.Post)
                .Include(e => e.Comment)
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (endorsement == null)
            {
                LogEndorsementNotFound(_logger, request.Id);
                throw new ArgumentException($"Endorsement com ID {request.Id} não encontrado.");
            }

            LogEndorsementFound(_logger, endorsement.Id, endorsement.Type);

            // Mapear para DTO
            var result = _mapper.Map<EndorsementDto>(endorsement);
            
            // Adicionar informações de display do tipo
            var typeInfo = EndorsementTypeHelper.GetTypeInfo(result.Type);
            result.TypeDisplayName = typeInfo.DisplayName;
            result.TypeIcon = typeInfo.Icon;

            return result;
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            LogEndorsementSearchError(_logger, request.Id, ex);
            throw;
        }
    }
}