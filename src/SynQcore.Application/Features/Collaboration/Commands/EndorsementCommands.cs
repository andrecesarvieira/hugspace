using MediatR;
using SynQcore.Application.Features.Collaboration.DTOs;

namespace SynQcore.Application.Features.Collaboration.Commands;

public record CreateEndorsementCommand : IRequest<EndorsementDto>
{
    public CreateEndorsementDto Data { get; set; } = null!;
    public Guid EndorserId { get; set; } // Usuário autenticado
}

public record UpdateEndorsementCommand : IRequest<EndorsementDto>
{
    public Guid Id { get; set; }
    public UpdateEndorsementDto Data { get; set; } = null!;
    public Guid UserId { get; set; } // Para verificação de autorização
}

public record DeleteEndorsementCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // Para verificação de autorização
}

public record ToggleEndorsementCommand : IRequest<EndorsementDto?>
{
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    public SynQcore.Domain.Entities.Communication.EndorsementType Type { get; set; }
    public Guid EndorserId { get; set; }
    public string? Context { get; set; }
}

public record BulkEndorsementCommand : IRequest<List<EndorsementDto>>
{
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    public List<SynQcore.Domain.Entities.Communication.EndorsementType> Types { get; set; } = new();
    public Guid EndorserId { get; set; }
    public string? Context { get; set; }
    public string? Note { get; set; }
}
