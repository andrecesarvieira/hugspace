using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Queries;

namespace SynQcore.Application.Features.Collaboration.Handlers;

/// <summary>
/// Handler para verificar se um usuário já endossou um conteúdo específico
/// </summary>
public partial class CheckUserEndorsementQueryHandler : IRequestHandler<CheckUserEndorsementQuery, EndorsementDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<CheckUserEndorsementQueryHandler> _logger;

    public CheckUserEndorsementQueryHandler(
        ISynQcoreDbContext context,
        ILogger<CheckUserEndorsementQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Verificando endorsement do usuário {UserId} para PostId={PostId}, CommentId={CommentId}, Type={EndorsementType}")]
    private static partial void LogCheckingUserEndorsement(ILogger logger, Guid userId, Guid? postId, Guid? commentId, string? endorsementType, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Endorsement encontrado: {EndorsementId} para usuário {UserId}")]
    private static partial void LogEndorsementFound(ILogger logger, Guid endorsementId, Guid userId, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Nenhum endorsement encontrado para usuário {UserId}")]
    private static partial void LogNoEndorsementFound(ILogger logger, Guid userId, Exception? exception);

    public async Task<EndorsementDto?> Handle(CheckUserEndorsementQuery request, CancellationToken cancellationToken)
    {
        LogCheckingUserEndorsement(_logger, request.UserId, request.PostId, request.CommentId, request.SpecificType?.ToString(), null);

        // Validar que pelo menos um alvo foi especificado
        if (request.PostId == null && request.CommentId == null)
        {
            throw new ArgumentException("É necessário especificar PostId ou CommentId para verificar endorsement.");
        }

        // Construir query baseada nos parâmetros fornecidos
        var query = _context.Endorsements
            .Include(e => e.Endorser)
            .Include(e => e.Post)
            .Include(e => e.Comment)
            .Where(e => e.EndorserId == request.UserId);

        // Filtrar por alvo específico
        if (request.PostId.HasValue)
        {
            query = query.Where(e => e.PostId == request.PostId.Value);
        }

        if (request.CommentId.HasValue)
        {
            query = query.Where(e => e.CommentId == request.CommentId.Value);
        }

        // Filtrar por tipo específico se fornecido
        if (request.SpecificType.HasValue)
        {
            query = query.Where(e => e.Type == request.SpecificType.Value);
        }

        // Buscar o primeiro endorsement que atenda aos critérios
        var endorsement = await query
            .OrderByDescending(e => e.EndorsedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (endorsement != null)
        {
            LogEndorsementFound(_logger, endorsement.Id, request.UserId, null);

            // Usar extension method para mapeamento
            return endorsement.ToEndorsementDto();
        }

        LogNoEndorsementFound(_logger, request.UserId, null);
        return null;
    }
}
