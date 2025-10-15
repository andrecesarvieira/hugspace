using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.Commands;

namespace SynQcore.Application.Features.Collaboration.Handlers;

public partial class DeleteEndorsementCommandHandler : IRequestHandler<DeleteEndorsementCommand>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<DeleteEndorsementCommandHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3021, Level = LogLevel.Information,
        Message = "Excluindo endorsement {EndorsementId} por usuário {UserId}")]
    private static partial void LogDeletingEndorsement(ILogger logger, Guid endorsementId, Guid userId);

    [LoggerMessage(EventId = 3022, Level = LogLevel.Information,
        Message = "Endorsement excluído com sucesso: {EndorsementId}")]
    private static partial void LogEndorsementDeleted(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 3023, Level = LogLevel.Warning,
        Message = "Endorsement não encontrado: {EndorsementId}")]
    private static partial void LogEndorsementNotFound(ILogger logger, Guid endorsementId);

    [LoggerMessage(EventId = 3024, Level = LogLevel.Warning,
        Message = "Usuario {UserId} não autorizado a excluir endorsement {EndorsementId} (proprietário: {OwnerId})")]
    private static partial void LogUnauthorizedDelete(ILogger logger, Guid userId, Guid endorsementId, Guid ownerId);

    [LoggerMessage(EventId = 3025, Level = LogLevel.Error,
        Message = "Erro ao excluir endorsement {EndorsementId}")]
    private static partial void LogEndorsementDeleteError(ILogger logger, Guid endorsementId, Exception ex);

    public DeleteEndorsementCommandHandler(
        ISynQcoreDbContext context,
        ILogger<DeleteEndorsementCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(DeleteEndorsementCommand request, CancellationToken cancellationToken)
    {
        LogDeletingEndorsement(_logger, request.Id, request.UserId);

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

            // Validar autorização - apenas o próprio autor pode excluir
            if (endorsement.EndorserId != request.UserId)
            {
                LogUnauthorizedDelete(_logger, request.UserId, request.Id, endorsement.EndorserId);
                throw new UnauthorizedAccessException("Apenas o autor do endorsement pode excluí-lo.");
            }

            // Soft delete - marcar como deletado (BaseEntity)
            endorsement.IsDeleted = true;
            endorsement.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            LogEndorsementDeleted(_logger, request.Id);
        }
        catch (Exception ex) when (!(ex is ArgumentException or UnauthorizedAccessException))
        {
            LogEndorsementDeleteError(_logger, request.Id, ex);
            throw;
        }
    }
}
