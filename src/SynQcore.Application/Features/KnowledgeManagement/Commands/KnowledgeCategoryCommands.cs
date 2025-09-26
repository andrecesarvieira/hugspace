using MediatR;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace SynQcore.Application.Features.KnowledgeManagement.Commands;

public class UpdateKnowledgeCategoryCommand : IRequest<KnowledgeCategoryDto>
{
    public Guid Id { get; set; }
    public UpdateKnowledgeCategoryDto Data { get; set; } = null!;
}

public class UpdateKnowledgeCategoryCommandHandler : IRequestHandler<UpdateKnowledgeCategoryCommand, KnowledgeCategoryDto>
{
    private readonly ISynQcoreDbContext _context;

    public UpdateKnowledgeCategoryCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<KnowledgeCategoryDto> Handle(UpdateKnowledgeCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.KnowledgeCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            throw new NotFoundException($"Categoria com ID {request.Id} não encontrada.");

        // Verificar se novo nome já existe (se foi alterado)
        if (!string.IsNullOrEmpty(request.Data.Name) && request.Data.Name != category.Name)
        {
            var existingCategory = await _context.KnowledgeCategories
                .FirstOrDefaultAsync(c => c.Name == request.Data.Name, cancellationToken);

            if (existingCategory != null)
                throw new ConflictException($"Categoria com nome '{request.Data.Name}' já existe.");
        }

        // Verificar categoria pai (se especificada e alterada)
        if (request.Data.ParentCategoryId.HasValue && request.Data.ParentCategoryId != category.ParentCategoryId)
        {
            // Verificar se não está tentando se tornar pai de si mesma ou criar ciclo
            if (request.Data.ParentCategoryId == category.Id)
                throw new ValidationException("Uma categoria não pode ser pai de si mesma.");

            var parentExists = await _context.KnowledgeCategories
                .AnyAsync(c => c.Id == request.Data.ParentCategoryId, cancellationToken);

            if (!parentExists)
                throw new NotFoundException("Categoria pai não encontrada.");
        }

        // Atualizar propriedades
        if (!string.IsNullOrEmpty(request.Data.Name))
            category.Name = request.Data.Name;

        if (!string.IsNullOrEmpty(request.Data.Description))
            category.Description = request.Data.Description;

        if (!string.IsNullOrEmpty(request.Data.Color))
            category.Color = request.Data.Color;

        if (!string.IsNullOrEmpty(request.Data.Icon))
            category.Icon = request.Data.Icon;

        if (request.Data.IsActive.HasValue)
            category.IsActive = request.Data.IsActive.Value;

        if (request.Data.ParentCategoryId != category.ParentCategoryId)
            category.ParentCategoryId = request.Data.ParentCategoryId;

        await _context.SaveChangesAsync(cancellationToken);

        return category.ToKnowledgeCategoryDto();
    }
}

public class DeleteKnowledgeCategoryCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteKnowledgeCategoryCommandHandler : IRequestHandler<DeleteKnowledgeCategoryCommand>
{
    private readonly ISynQcoreDbContext _context;

    public DeleteKnowledgeCategoryCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteKnowledgeCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.KnowledgeCategories
            .Include(c => c.Posts)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            throw new NotFoundException($"Categoria com ID {request.Id} não encontrada.");

        // Verificar se tem subcategorias
        if (category.SubCategories.Count > 0)
            throw new ValidationException("Não é possível excluir categoria que possui subcategorias.");

        // Verificar se tem posts associados
        if (category.Posts.Count > 0)
            throw new ValidationException("Não é possível excluir categoria que possui posts associados.");

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}