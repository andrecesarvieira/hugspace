using MediatR;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace SynQcore.Application.Features.KnowledgeManagement.Commands;

/// <summary>
/// Command para atualizar uma categoria de conhecimento existente
/// </summary>
public record UpdateKnowledgeCategoryCommand : IRequest<KnowledgeCategoryDto>
{
    /// <summary>
    /// ID da categoria a ser atualizada
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Dados atualizados da categoria
    /// </summary>
    public UpdateKnowledgeCategoryDto Data { get; init; } = default!;
}

/// <summary>
/// Handler para processar comando de atualização de categoria de conhecimento
/// </summary>
public class UpdateKnowledgeCategoryCommandHandler : IRequestHandler<UpdateKnowledgeCategoryCommand, KnowledgeCategoryDto>
{
    private readonly ISynQcoreDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância do UpdateKnowledgeCategoryCommandHandler
    /// </summary>
    /// <param name="context">Contexto do banco de dados</param>
    public UpdateKnowledgeCategoryCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Processa o comando de atualização de categoria de conhecimento
    /// </summary>
    /// <param name="request">Comando de atualização</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>DTO da categoria atualizada</returns>
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

/// <summary>
/// Command para excluir uma categoria de conhecimento
/// </summary>
public record DeleteKnowledgeCategoryCommand : IRequest<bool>
{
    /// <summary>
    /// ID da categoria a ser excluída
    /// </summary>
    public Guid Id { get; init; }
}

/// <summary>
/// Handler para processar comando de exclusão de categoria de conhecimento
/// </summary>
public class DeleteKnowledgeCategoryCommandHandler : IRequestHandler<DeleteKnowledgeCategoryCommand, bool>
{
    private readonly ISynQcoreDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância do DeleteKnowledgeCategoryCommandHandler
    /// </summary>
    /// <param name="context">Contexto do banco de dados</param>
    public DeleteKnowledgeCategoryCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Processa o comando de exclusão de categoria de conhecimento
    /// </summary>
    /// <param name="request">Comando de exclusão</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se a exclusão foi bem-sucedida</returns>
    public async Task<bool> Handle(DeleteKnowledgeCategoryCommand request, CancellationToken cancellationToken)
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

        return true;
    }
}
