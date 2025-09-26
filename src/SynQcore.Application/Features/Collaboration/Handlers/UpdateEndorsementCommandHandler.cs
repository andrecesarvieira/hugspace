using SynQcore.Application.Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.Commands;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Helpers;

namespace SynQcore.Application.Features.Collaboration.Handlers;

/// <summary>
/// Handler para atualização de endorsement corporativo com controle de autorização
/// </summary>
public partial class UpdateEndorsementCommandHandler : IRequestHandler<UpdateEndorsementCommand, EndorsementDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UpdateEndorsementCommandHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3011, Level = LogLevel.Information,
        Message = "Atualizando endorsement {EndorsementId} por usuário {UserId}")]
    private static partial void LogUpdatingEndorsement(ILogger logger, Guid endorsementId, Guid userId);

    [LoggerMessage(EventId = 3012, Level = LogLevel.Information,
        Message = "Endorsement atualizado com sucesso: {EndorsementId}")]
    private static partial void LogEndorsementUpdated(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 3013, Level = LogLevel.Warning,
        Message = "Endorsement não encontrado: {EndorsementId}")]
    private static partial void LogEndorsementNotFound(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 3014, Level = LogLevel.Warning,
        Message = "Usuario {UserId} não autorizado a atualizar endorsement {EndorsementId} (proprietário: {OwnerId})")]
    private static partial void LogUnauthorizedUpdate(ILogger logger, Guid userId, Guid endorsementId, Guid ownerId);

    [LoggerMessage(EventId = 3015, Level = LogLevel.Warning,
        Message = "Tentativa de atualizar endorsement para tipo duplicado {Type} por usuário {UserId} em conteúdo {ContentType}:{ContentId}")]
    private static partial void LogDuplicateTypeUpdate(ILogger logger, SynQcore.Domain.Entities.Communication.EndorsementType type, Guid userId, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3016, Level = LogLevel.Error,
        Message = "Erro ao atualizar endorsement {EndorsementId}")]
    private static partial void LogEndorsementUpdateError(ILogger logger, Guid endorsementId, Exception ex);

    public UpdateEndorsementCommandHandler(
        ISynQcoreDbContext context, 
        
        ILogger<UpdateEndorsementCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EndorsementDto> Handle(UpdateEndorsementCommand request, CancellationToken cancellationToken)
    {
        LogUpdatingEndorsement(_logger, request.Id, request.UserId);

        try
        {
            // Buscar endorsement existente
            var endorsement = await _context.Endorsements
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (endorsement == null)
            {
                LogEndorsementNotFound(_logger, request.Id);
                throw new ArgumentException($"Endorsement com ID {request.Id} não encontrado.");
            }

            // Validar autorização - apenas o próprio autor pode atualizar
            if (endorsement.EndorserId != request.UserId)
            {
                LogUnauthorizedUpdate(_logger, request.UserId, request.Id, endorsement.EndorserId);
                throw new UnauthorizedAccessException("Apenas o autor do endorsement pode atualizá-lo.");
            }

            var data = request.Data;

            // Se está mudando o tipo, verificar se não causará duplicata
            if (data.Type.HasValue && data.Type.Value != endorsement.Type)
            {
                var contentType = endorsement.PostId.HasValue ? "Post" : "Comment";
                var contentId = endorsement.PostId ?? endorsement.CommentId!.Value;

                var duplicateExists = await _context.Endorsements
                    .AnyAsync(e => e.Id != request.Id && // Excluir o próprio endorsement
                              e.EndorserId == endorsement.EndorserId && 
                              e.Type == data.Type.Value &&
                              ((endorsement.PostId.HasValue && e.PostId == endorsement.PostId) ||
                               (endorsement.CommentId.HasValue && e.CommentId == endorsement.CommentId)), 
                              cancellationToken);

                if (duplicateExists)
                {
                    LogDuplicateTypeUpdate(_logger, data.Type.Value, request.UserId, contentType, contentId);
                    throw new ArgumentException($"Usuário já possui endorsement do tipo {data.Type.Value} para este conteúdo.");
                }
            }

            // Aplicar atualizações
            if (data.Type.HasValue)
                endorsement.Type = data.Type.Value;

            if (!string.IsNullOrEmpty(data.Note))
                endorsement.Note = data.Note.Trim();

            if (data.IsPublic.HasValue)
                endorsement.IsPublic = data.IsPublic.Value;

            if (!string.IsNullOrEmpty(data.Context))
                endorsement.Context = data.Context.Trim();

            // Atualizar timestamp de modificação
            endorsement.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            LogEndorsementUpdated(_logger, request.Id);

            // Buscar endorsement completo para retorno
            var updatedEndorsement = await _context.Endorsements
                .Include(e => e.Endorser)
                .Include(e => e.Post)
                .Include(e => e.Comment)
                .FirstAsync(e => e.Id == request.Id, cancellationToken);

            // Mapear para DTO com informações de display
            var result = updatedEndorsement.ToEndorsementDto();
            
            // Adicionar informações de display do tipo
            var typeInfo = EndorsementTypeHelper.GetTypeInfo(result.Type);
            result.TypeDisplayName = typeInfo.DisplayName;
            result.TypeIcon = typeInfo.Icon;

            return result;
        }
        catch (Exception ex) when (!(ex is ArgumentException or UnauthorizedAccessException))
        {
            LogEndorsementUpdateError(_logger, request.Id, ex);
            throw;
        }
    }
}