using MediatR;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Application.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace SynQcore.Application.Features.KnowledgeManagement.Commands;

/// <summary>
/// Command para criar uma nova categoria de conhecimento
/// </summary>
public record CreateKnowledgeCategoryCommand : IRequest<KnowledgeCategoryDto>
{
    /// <summary>
    /// Dados da categoria a ser criada
    /// </summary>
    public CreateKnowledgeCategoryDto Data { get; init; } = default!;
}

/// <summary>
/// Handler para processar comando de criação de categoria de conhecimento
/// </summary>
public class CreateKnowledgeCategoryCommandHandler : IRequestHandler<CreateKnowledgeCategoryCommand, KnowledgeCategoryDto>
{
    private readonly ISynQcoreDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância do CreateKnowledgeCategoryCommandHandler
    /// </summary>
    /// <param name="context">Contexto do banco de dados</param>
    public CreateKnowledgeCategoryCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Processa o comando de criação de categoria de conhecimento
    /// </summary>
    /// <param name="request">Comando de criação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>DTO da categoria criada</returns>
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
