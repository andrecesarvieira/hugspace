using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.Commands;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Helpers;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.Handlers;

/// <summary>
/// Handler para toggle de endorsement (adicionar/remover rapidamente)
/// </summary>
public partial class ToggleEndorsementCommandHandler : IRequestHandler<ToggleEndorsementCommand, EndorsementDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ToggleEndorsementCommandHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3041, Level = LogLevel.Information,
        Message = "Toggle endorsement tipo {Type} por usuário {UserId} em {ContentType}:{ContentId}")]
    private static partial void LogTogglingEndorsement(ILogger logger, EndorsementType type, Guid userId, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3042, Level = LogLevel.Information,
        Message = "Endorsement criado via toggle: {EndorsementId} por usuário {UserId}")]
    private static partial void LogEndorsementToggleCreated(ILogger logger, Guid endorsementId, Guid userId);

    [LoggerMessage(EventId = 3043, Level = LogLevel.Information,
        Message = "Endorsement removido via toggle: {EndorsementId} por usuário {UserId}")]
    private static partial void LogEndorsementToggleRemoved(ILogger logger, Guid endorsementId, Guid userId);

    [LoggerMessage(EventId = 3044, Level = LogLevel.Warning,
        Message = "Conteúdo não encontrado para toggle - {ContentType}:{ContentId}")]
    private static partial void LogToggleContentNotFound(ILogger logger, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3045, Level = LogLevel.Warning,
        Message = "Usuario {UserId} tentou auto-endossar próprio conteúdo via toggle {ContentType}:{ContentId}")]
    private static partial void LogToggleSelfEndorsementAttempt(ILogger logger, Guid userId, string contentType, Guid contentId);

    [LoggerMessage(EventId = 3046, Level = LogLevel.Error,
        Message = "Erro no toggle de endorsement por usuário {UserId}")]
    private static partial void LogToggleError(ILogger logger, Guid userId, Exception ex);

    public ToggleEndorsementCommandHandler(
        ISynQcoreDbContext context, 
        IMapper mapper, 
        ILogger<ToggleEndorsementCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EndorsementDto?> Handle(ToggleEndorsementCommand request, CancellationToken cancellationToken)
    {
        var contentType = request.PostId.HasValue ? "Post" : "Comment";
        var contentId = request.PostId ?? request.CommentId!.Value;

        LogTogglingEndorsement(_logger, request.Type, request.EndorserId, contentType, contentId);

        try
        {
            // Validar se o conteúdo existe e obter o autor
            Guid contentAuthorId;
            if (request.PostId.HasValue)
            {
                var post = await _context.Posts
                    .Where(p => p.Id == request.PostId.Value)
                    .Select(p => new { p.Id, p.AuthorId })
                    .FirstOrDefaultAsync(cancellationToken);

                if (post == null)
                {
                    LogToggleContentNotFound(_logger, "Post", request.PostId.Value);
                    throw new ArgumentException($"Post com ID {request.PostId} não encontrado.");
                }
                contentAuthorId = post.AuthorId;
            }
            else
            {
                var comment = await _context.Comments
                    .Where(c => c.Id == request.CommentId!.Value)
                    .Select(c => new { c.Id, c.AuthorId })
                    .FirstOrDefaultAsync(cancellationToken);

                if (comment == null)
                {
                    LogToggleContentNotFound(_logger, "Comment", request.CommentId!.Value);
                    throw new ArgumentException($"Comment com ID {request.CommentId} não encontrado.");
                }
                contentAuthorId = comment.AuthorId;
            }

            // Validar auto-endorsement
            if (contentAuthorId == request.EndorserId)
            {
                LogToggleSelfEndorsementAttempt(_logger, request.EndorserId, contentType, contentId);
                throw new ArgumentException("Não é possível endossar o próprio conteúdo.");
            }

            // Verificar se já existe endorsement do mesmo tipo
            var existingEndorsement = await _context.Endorsements
                .Where(e => e.EndorserId == request.EndorserId && 
                           e.Type == request.Type &&
                           ((request.PostId.HasValue && e.PostId == request.PostId) ||
                            (request.CommentId.HasValue && e.CommentId == request.CommentId)))
                .FirstOrDefaultAsync(cancellationToken);

            if (existingEndorsement != null)
            {
                // REMOVER endorsement existente (toggle OFF)
                existingEndorsement.IsDeleted = true;
                existingEndorsement.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                LogEndorsementToggleRemoved(_logger, existingEndorsement.Id, request.EndorserId);
                return null; // Indica que foi removido
            }
            else
            {
                // CRIAR novo endorsement (toggle ON)
                var newEndorsement = new Endorsement
                {
                    PostId = request.PostId,
                    CommentId = request.CommentId,
                    EndorserId = request.EndorserId,
                    Type = request.Type,
                    IsPublic = true, // Toggle sempre público por padrão
                    Context = request.Context?.Trim(),
                    EndorsedAt = DateTime.UtcNow
                };

                _context.Endorsements.Add(newEndorsement);
                await _context.SaveChangesAsync(cancellationToken);

                LogEndorsementToggleCreated(_logger, newEndorsement.Id, request.EndorserId);

                // Buscar endorsement completo para retorno
                var createdEndorsement = await _context.Endorsements
                    .Include(e => e.Endorser)
                    .Include(e => e.Post)
                    .Include(e => e.Comment)
                    .FirstAsync(e => e.Id == newEndorsement.Id, cancellationToken);

                // Mapear para DTO
                var result = _mapper.Map<EndorsementDto>(createdEndorsement);
                
                // Adicionar informações de display do tipo
                var typeInfo = EndorsementTypeHelper.GetTypeInfo(result.Type);
                result.TypeDisplayName = typeInfo.DisplayName;
                result.TypeIcon = typeInfo.Icon;

                return result;
            }
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            LogToggleError(_logger, request.EndorserId, ex);
            throw;
        }
    }
}