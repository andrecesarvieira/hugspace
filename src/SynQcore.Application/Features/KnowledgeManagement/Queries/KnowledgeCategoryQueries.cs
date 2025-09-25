using MediatR;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SynQcore.Application.Features.KnowledgeManagement.Queries;

public class GetKnowledgeCategoriesQuery : IRequest<List<KnowledgeCategoryDto>>
{
    public bool IncludeInactive { get; set; }
    public bool IncludeHierarchy { get; set; } = true;
}

public class GetKnowledgeCategoriesQueryHandler : IRequestHandler<GetKnowledgeCategoriesQuery, List<KnowledgeCategoryDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetKnowledgeCategoriesQueryHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<KnowledgeCategoryDto>> Handle(GetKnowledgeCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.KnowledgeCategories
            .Include(c => c.ParentCategory)
            .AsQueryable();

        if (!request.IncludeInactive)
            query = query.Where(c => c.IsActive);

        if (request.IncludeHierarchy)
            query = query.Include(c => c.SubCategories.Where(sc => request.IncludeInactive || sc.IsActive));

        var categories = await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<KnowledgeCategoryDto>>(categories);

        // Adicionar contagem de posts para cada categoria
        var categoryIds = categories.Select(c => c.Id).ToList();
        var postCounts = await _context.Posts
            .Where(p => p.CategoryId.HasValue && categoryIds.Contains(p.CategoryId.Value))
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        foreach (var category in result)
        {
            var count = postCounts.FirstOrDefault(pc => pc.CategoryId == category.Id);
            category.PostsCount = count?.Count ?? 0;
        }

        return result;
    }
}

public class GetKnowledgeCategoryByIdQuery : IRequest<KnowledgeCategoryDto>
{
    public Guid Id { get; set; }
}

public class GetKnowledgeCategoryByIdQueryHandler : IRequestHandler<GetKnowledgeCategoryByIdQuery, KnowledgeCategoryDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetKnowledgeCategoryByIdQueryHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<KnowledgeCategoryDto> Handle(GetKnowledgeCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.KnowledgeCategories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories.Where(sc => sc.IsActive))
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            throw new NotFoundException($"Categoria com ID {request.Id} não encontrada.");

        var result = _mapper.Map<KnowledgeCategoryDto>(category);

        // Adicionar contagem de posts
        result.PostsCount = await _context.Posts
            .CountAsync(p => p.CategoryId == category.Id, cancellationToken);

        return result;
    }
}