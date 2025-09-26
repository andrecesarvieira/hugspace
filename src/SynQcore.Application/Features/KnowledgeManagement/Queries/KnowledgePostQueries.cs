using MediatR;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Common.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace SynQcore.Application.Features.KnowledgeManagement.Queries;

// Query para buscar artigos com paginação
public class GetKnowledgePostsQuery : IRequest<PagedResult<KnowledgePostDto>>
{
    public KnowledgePostSearchDto SearchRequest { get; set; } = null!;
}

public class GetKnowledgePostsQueryHandler : IRequestHandler<GetKnowledgePostsQuery, PagedResult<KnowledgePostDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetKnowledgePostsQueryHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<KnowledgePostDto>> Handle(GetKnowledgePostsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Department)
            .Include(p => p.Team)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        var searchRequest = request.SearchRequest;

        // Aplicar filtros
        if (!string.IsNullOrEmpty(searchRequest.SearchTerm))
        {
            var searchTerm = searchRequest.SearchTerm.ToLower(CultureInfo.InvariantCulture);
            query = query.Where(p => 
                p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (p.Summary != null && p.Summary.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        if (searchRequest.Type.HasValue)
            query = query.Where(p => p.Type == searchRequest.Type.Value);

        if (searchRequest.Status.HasValue)
            query = query.Where(p => p.Status == searchRequest.Status.Value);

        if (searchRequest.Visibility.HasValue)
            query = query.Where(p => p.Visibility == searchRequest.Visibility.Value);

        if (searchRequest.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == searchRequest.CategoryId.Value);

        if (searchRequest.AuthorId.HasValue)
            query = query.Where(p => p.AuthorId == searchRequest.AuthorId.Value);

        if (searchRequest.DepartmentId.HasValue)
            query = query.Where(p => p.DepartmentId == searchRequest.DepartmentId.Value);

        if (searchRequest.TeamId.HasValue)
            query = query.Where(p => p.TeamId == searchRequest.TeamId.Value);

        if (searchRequest.TagIds != null && searchRequest.TagIds.Count > 0)
        {
            query = query.Where(p => p.PostTags.Any(pt => searchRequest.TagIds.Contains(pt.TagId)));
        }

        if (searchRequest.CreatedAfter.HasValue)
            query = query.Where(p => p.CreatedAt >= searchRequest.CreatedAfter.Value);

        if (searchRequest.CreatedBefore.HasValue)
            query = query.Where(p => p.CreatedAt <= searchRequest.CreatedBefore.Value);

        // Aplicar ordenação
        query = searchRequest.SortBy?.ToLower(CultureInfo.InvariantCulture) switch
        {
            "title" => searchRequest.SortDescending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
            "createdat" => searchRequest.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            "updatedat" => searchRequest.SortDescending ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt),
            "viewcount" => searchRequest.SortDescending ? query.OrderByDescending(p => p.ViewCount) : query.OrderBy(p => p.ViewCount),
            "likecount" => searchRequest.SortDescending ? query.OrderByDescending(p => p.LikeCount) : query.OrderBy(p => p.LikeCount),
            _ => searchRequest.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .Skip((searchRequest.Page - 1) * searchRequest.PageSize)
            .Take(searchRequest.PageSize)
            .ToListAsync(cancellationToken);

        var postDtos = _mapper.Map<List<KnowledgePostDto>>(posts);

        return new PagedResult<KnowledgePostDto>
        {
            Items = postDtos,
            TotalCount = totalCount,
            Page = searchRequest.Page,
            PageSize = searchRequest.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / searchRequest.PageSize)
        };
    }
}

// Query para buscar artigo por ID
public class GetKnowledgePostByIdQuery : IRequest<KnowledgePostDto>
{
    public Guid Id { get; set; }
    public bool IncrementViewCount { get; set; } = true;
}

public class GetKnowledgePostByIdQueryHandler : IRequestHandler<GetKnowledgePostByIdQuery, KnowledgePostDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetKnowledgePostByIdQueryHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<KnowledgePostDto> Handle(GetKnowledgePostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Department)
            .Include(p => p.Team)
            .Include(p => p.ParentPost)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .Include(p => p.Versions.Where(v => !v.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (post == null)
            throw new NotFoundException($"Artigo com ID {request.Id} não encontrado.");

        // Incrementar contador de visualizações
        if (request.IncrementViewCount)
        {
            post.ViewCount++;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return _mapper.Map<KnowledgePostDto>(post);
    }
}

// Query para buscar versões de um artigo
public class GetKnowledgePostVersionsQuery : IRequest<List<KnowledgePostDto>>
{
    public Guid ParentPostId { get; set; }
}

public class GetKnowledgePostVersionsQueryHandler : IRequestHandler<GetKnowledgePostVersionsQuery, List<KnowledgePostDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetKnowledgePostVersionsQueryHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<KnowledgePostDto>> Handle(GetKnowledgePostVersionsQuery request, CancellationToken cancellationToken)
    {
        // Verificar se post pai existe
        var parentExists = await _context.Posts
            .AnyAsync(p => p.Id == request.ParentPostId && !p.IsDeleted, cancellationToken);

        if (!parentExists)
            throw new NotFoundException($"Post pai com ID {request.ParentPostId} não encontrado.");

        var versions = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .Where(p => p.ParentPostId == request.ParentPostId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<KnowledgePostDto>>(versions);
    }
}

// Query para buscar artigos por categoria
public class GetKnowledgePostsByCategoryQuery : IRequest<PagedResult<KnowledgePostDto>>
{
    public Guid CategoryId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

public class GetKnowledgePostsByCategoryQueryHandler : IRequestHandler<GetKnowledgePostsByCategoryQuery, PagedResult<KnowledgePostDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;

    public GetKnowledgePostsByCategoryQueryHandler(ISynQcoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<KnowledgePostDto>> Handle(GetKnowledgePostsByCategoryQuery request, CancellationToken cancellationToken)
    {
        // Verificar se categoria existe
        var categoryExists = await _context.KnowledgeCategories
            .AnyAsync(c => c.Id == request.CategoryId && !c.IsDeleted, cancellationToken);

        if (!categoryExists)
            throw new NotFoundException($"Categoria com ID {request.CategoryId} não encontrada.");

        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Department)
            .Include(p => p.Team)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .Where(p => p.CategoryId == request.CategoryId && !p.IsDeleted)
            .AsQueryable();

        // Aplicar ordenação
        query = request.SortBy?.ToLower(CultureInfo.InvariantCulture) switch
        {
            "title" => request.SortDescending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
            "createdat" => request.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            "updatedat" => request.SortDescending ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt),
            "viewcount" => request.SortDescending ? query.OrderByDescending(p => p.ViewCount) : query.OrderBy(p => p.ViewCount),
            _ => request.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var postDtos = _mapper.Map<List<KnowledgePostDto>>(posts);

        return new PagedResult<KnowledgePostDto>
        {
            Items = postDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}