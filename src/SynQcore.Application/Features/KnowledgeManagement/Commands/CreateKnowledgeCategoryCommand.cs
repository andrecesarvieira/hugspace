using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.KnowledgeManagement.Commands;

public record CreateKnowledgeCategoryCommand : IRequest<KnowledgeCategoryDto>
{
    public CreateKnowledgeCategoryDto Data { get; init; } = default!;
}

public class CreateKnowledgeCategoryCommandHandler : IRequestHandler<CreateKnowledgeCategoryCommand, KnowledgeCategoryDto>
{
    private readonly ISynQcoreDbContext _context;

    public CreateKnowledgeCategoryCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<KnowledgeCategoryDto> Handle(CreateKnowledgeCategoryCommand request, CancellationToken cancellationToken)
    {
        // Verificar se nome já existe
        var existingCategory = await _context.KnowledgeCategories
            .FirstOrDefaultAsync(c => c.Name == request.Data.Name, cancellationToken);

        if (existingCategory != null)
            throw new ConflictException($"Categoria com nome '{request.Data.Name}' já existe.");

        // Verificar se categoria pai existe (se especificada)
        if (request.Data.ParentCategoryId.HasValue)
        {
            var parentExists = await _context.KnowledgeCategories
                .AnyAsync(c => c.Id == request.Data.ParentCategoryId, cancellationToken);

            if (!parentExists)
                throw new NotFoundException("Categoria pai não encontrada.");
        }

        var category = new KnowledgeCategory
        {
            Name = request.Data.Name,
            Description = request.Data.Description,
            Color = request.Data.Color,
            Icon = request.Data.Icon,
            IsActive = request.Data.IsActive,
            ParentCategoryId = request.Data.ParentCategoryId
        };

        _context.KnowledgeCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.ToKnowledgeCategoryDto();
    }
}
