using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.KnowledgeManagement.Commands;

public class CreateTagCommand : IRequest<TagDto>
{
    public CreateTagDto Data { get; set; } = null!;
}

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
{
    private readonly ISynQcoreDbContext _context;

    public CreateTagCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        // Verificar se nome já existe
        var existingTag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Name == request.Data.Name, cancellationToken);

        if (existingTag != null)
            throw new ConflictException($"Tag com nome '{request.Data.Name}' já existe.");

        var tag = new Tag
        {
            Name = request.Data.Name,
            Description = request.Data.Description,
            Type = request.Data.Type,
            Color = request.Data.Color
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return tag.ToTagDto();
    }
}

public class UpdateTagCommand : IRequest<TagDto>
{
    public Guid Id { get; set; }
    public UpdateTagDto Data { get; set; } = null!;
}

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, TagDto>
{
    private readonly ISynQcoreDbContext _context;

    public UpdateTagCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<TagDto> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag == null)
            throw new NotFoundException($"Tag com ID {request.Id} não encontrada.");

        // Verificar se novo nome já existe (se foi alterado)
        if (!string.IsNullOrEmpty(request.Data.Name) && request.Data.Name != tag.Name)
        {
            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name == request.Data.Name, cancellationToken);

            if (existingTag != null)
                throw new ConflictException($"Tag com nome '{request.Data.Name}' já existe.");
        }

        // Atualizar propriedades
        if (!string.IsNullOrEmpty(request.Data.Name))
            tag.Name = request.Data.Name;

        if (request.Data.Description != null)
            tag.Description = request.Data.Description;

        if (request.Data.Type.HasValue)
            tag.Type = request.Data.Type.Value;

        if (!string.IsNullOrEmpty(request.Data.Color))
            tag.Color = request.Data.Color;

        await _context.SaveChangesAsync(cancellationToken);

        return tag.ToTagDto();
    }
}

public class DeleteTagCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly ISynQcoreDbContext _context;

    public DeleteTagCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .Include(t => t.PostTags)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag == null)
            throw new NotFoundException($"Tag com ID {request.Id} não encontrada.");

        // Verificar se tem posts associados
        if (tag.PostTags.Count > 0)
            throw new ValidationException("Não é possível excluir tag que está associada a posts.");

        tag.IsDeleted = true;
        tag.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
