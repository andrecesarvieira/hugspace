using SynQcore.Application.Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.Commands;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Helpers;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.Handlers;

public partial class CreateEndorsementCommandHandler : IRequestHandler<CreateEndorsementCommand, EndorsementDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<CreateEndorsementCommandHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3001, Level = LogLevel.Information,
        Message = "Criando endorsement tipo {Type} por usuário {UserId} para conteúdo {ContentType}:{ContentId}")]
    private static partial void LogCreatingEndorsement(ILogger logger, EndorsementType type, Guid userId, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3002, Level = LogLevel.Information,
        Message = "Endorsement criado com sucesso: {EndorsementId} por usuário {UserId}")]
    private static partial void LogEndorsementCreated(ILogger logger, Guid endorsementId, Guid userId);

    [LoggerMessage(EventId = 3003, Level = LogLevel.Warning,
        Message = "Conteúdo não encontrado para endorsement - {ContentType}:{ContentId}")]
    private static partial void LogContentNotFound(ILogger logger, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3004, Level = LogLevel.Warning,
        Message = "Usuario {UserId} já possui endorsement tipo {Type} para {ContentType}:{ContentId}")]
    private static partial void LogDuplicateEndorsement(ILogger logger, Guid userId, EndorsementType type, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3005, Level = LogLevel.Warning,
        Message = "Usuario {UserId} tentou auto-endossar próprio conteúdo {ContentType}:{ContentId}")]
    private static partial void LogSelfEndorsementAttempt(ILogger logger, Guid userId, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3006, Level = LogLevel.Error,
        Message = "Erro ao criar endorsement por usuário {UserId}")]
    private static partial void LogEndorsementCreationError(ILogger logger, Guid userId, Exception ex);

    public CreateEndorsementCommandHandler(
        ISynQcoreDbContext context, 
        ILogger<CreateEndorsementCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EndorsementDto> Handle(CreateEndorsementCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;
        var contentType = data.PostId.HasValue ? "Post" : "Comment";
        var contentId = data.PostId ?? data.CommentId!.Value;

        LogCreatingEndorsement(_logger, data.Type, request.EndorserId, contentType, contentId);

        try
        {
            // Validar se o conteúdo existe e obter o autor
            Guid contentAuthorId;
            if (data.PostId.HasValue)
            {
                var post = await _context.Posts
                    .Where(p => p.Id == data.PostId.Value)
                    .Select(p => new { p.Id, p.AuthorId })
                    .FirstOrDefaultAsync(cancellationToken);

                if (post == null)
                {
                    LogContentNotFound(_logger, "Post", data.PostId.Value);
                    throw new ArgumentException($"Post com ID {data.PostId} não encontrado.");
                }
                contentAuthorId = post.AuthorId;
            }
            else
            {
                var comment = await _context.Comments
                    .Where(c => c.Id == data.CommentId!.Value)
                    .Select(c => new { c.Id, c.AuthorId })
                    .FirstOrDefaultAsync(cancellationToken);

                if (comment == null)
                {
                    LogContentNotFound(_logger, "Comment", data.CommentId!.Value);
                    throw new ArgumentException($"Comment com ID {data.CommentId} não encontrado.");
                }
                contentAuthorId = comment.AuthorId;
            }

            // Validar auto-endorsement (política corporativa: não pode endossar próprio conteúdo)
            if (contentAuthorId == request.EndorserId)
            {
                LogSelfEndorsementAttempt(_logger, request.EndorserId, contentType, contentId);
                throw new ArgumentException("Não é possível endossar o próprio conteúdo.");
            }

            // Verificar se já existe endorsement do mesmo tipo pelo mesmo usuário
            var existingEndorsement = await _context.Endorsements
                .Where(e => e.EndorserId == request.EndorserId && 
                           e.Type == data.Type &&
                           ((data.PostId.HasValue && e.PostId == data.PostId) ||
                            (data.CommentId.HasValue && e.CommentId == data.CommentId)))
                .FirstOrDefaultAsync(cancellationToken);

            if (existingEndorsement != null)
            {
                LogDuplicateEndorsement(_logger, request.EndorserId, data.Type, contentType, contentId);
                throw new ArgumentException($"Usuário já possui endorsement do tipo {data.Type} para este conteúdo.");
            }

            // Criar endorsement
            var endorsement = new Endorsement
            {
                PostId = data.PostId,
                CommentId = data.CommentId,
                EndorserId = request.EndorserId,
                Type = data.Type,
                Note = data.Note?.Trim(),
                IsPublic = data.IsPublic,
                Context = data.Context?.Trim(),
                EndorsedAt = DateTime.UtcNow
            };

            _context.Endorsements.Add(endorsement);
            await _context.SaveChangesAsync(cancellationToken);

            LogEndorsementCreated(_logger, endorsement.Id, request.EndorserId);

            // Buscar endorsement completo com dados relacionados para retorno
            var createdEndorsement = await _context.Endorsements
                .Include(e => e.Endorser)
                .Include(e => e.Post)
                .Include(e => e.Comment)
                .FirstAsync(e => e.Id == endorsement.Id, cancellationToken);

            // Mapear para DTO com informações de display
            var result = createdEndorsement.ToEndorsementDto();
            
            // Adicionar informações de display do tipo
            var typeInfo = EndorsementTypeHelper.GetTypeInfo(result.Type);
            result.TypeDisplayName = typeInfo.DisplayName;
            result.TypeIcon = typeInfo.Icon;

            return result;
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            LogEndorsementCreationError(_logger, request.EndorserId, ex);
            throw;
        }
    }
}