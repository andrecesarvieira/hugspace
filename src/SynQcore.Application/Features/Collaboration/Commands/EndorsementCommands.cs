using MediatR;
using SynQcore.Application.Features.Collaboration.DTOs;

namespace SynQcore.Application.Features.Collaboration.Commands;

/// <summary>
/// Command para criar novo endorsement corporativo
/// </summary>
public record CreateEndorsementCommand : IRequest<EndorsementDto>
{
    public CreateEndorsementDto Data { get; set; } = null!;
    public Guid EndorserId { get; set; } // Usuário autenticado
}

/// <summary>
/// Command para atualizar endorsement existente
/// </summary>
public record UpdateEndorsementCommand : IRequest<EndorsementDto>
{
    public Guid Id { get; set; }
    public UpdateEndorsementDto Data { get; set; } = null!;
    public Guid UserId { get; set; } // Para verificação de autorização
}

/// <summary>
/// Command para remover endorsement
/// </summary>
public record DeleteEndorsementCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // Para verificação de autorização
}

/// <summary>
/// Command para endorsar/desendossar rapidamente (toggle)
/// </summary>
public record ToggleEndorsementCommand : IRequest<EndorsementDto?>
{
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    public SynQcore.Domain.Entities.Communication.EndorsementType Type { get; set; }
    public Guid EndorserId { get; set; }
    public string? Context { get; set; }
}

/// <summary>
/// Command para endorsement em massa (múltiplos tipos no mesmo conteúdo)
/// </summary>
public record BulkEndorsementCommand : IRequest<List<EndorsementDto>>
{
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    public List<SynQcore.Domain.Entities.Communication.EndorsementType> Types { get; set; } = new();
    public Guid EndorserId { get; set; }
    public string? Context { get; set; }
    public string? Note { get; set; }
}