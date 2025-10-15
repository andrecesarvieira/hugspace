using MediatR;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.KnowledgeManagement.Queries;

public class GetTagsQuery : IRequest<List<TagDto>>
{
    public TagType? Type { get; set; }
    public string? SearchTerm { get; set; }
    public int? MinUsageCount { get; set; }
    public string? SortBy { get; set; } = "Name";
    public bool SortDescending { get; set; }
}

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, List<TagDto>>
{
    private readonly ISynQcoreDbContext _context;

    public GetTagsQueryHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tags.AsQueryable();

        // Filtros
        if (request.Type.HasValue)
            query = query.Where(t => t.Type == request.Type);

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(t => t.Name.Contains(request.SearchTerm) ||
                                   (t.Description != null && t.Description.Contains(request.SearchTerm)));

        if (request.MinUsageCount.HasValue)
            query = query.Where(t => t.UsageCount >= request.MinUsageCount);

        // Ordenação
        query = request.SortBy?.ToLower(System.Globalization.CultureInfo.InvariantCulture) switch
        {
            "name" => request.SortDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
            "usagecount" => request.SortDescending ? query.OrderByDescending(t => t.UsageCount) : query.OrderBy(t => t.UsageCount),
            "createdat" => request.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
            _ => query.OrderBy(t => t.Name)
        };

        var tags = await query.ToListAsync(cancellationToken);

        return tags.ToTagDtos();
    }
}

public class GetTagByIdQuery : IRequest<TagDto>
{
    public Guid Id { get; set; }
}

public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto>
{
    private readonly ISynQcoreDbContext _context;

    public GetTagByIdQueryHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag == null)
            throw new NotFoundException($"Tag com ID {request.Id} não encontrada.");

        return tag.ToTagDto();
    }
}

public class GetPopularTagsQuery : IRequest<List<TagDto>>
{
    public int Count { get; set; } = 20;
    public TagType? Type { get; set; }
}

public class GetPopularTagsQueryHandler : IRequestHandler<GetPopularTagsQuery, List<TagDto>>
{
    private readonly ISynQcoreDbContext _context;

    public GetPopularTagsQueryHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<TagDto>> Handle(GetPopularTagsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tags.AsQueryable();

        if (request.Type.HasValue)
            query = query.Where(t => t.Type == request.Type);

        var tags = await query
            .OrderByDescending(t => t.UsageCount)
            .ThenBy(t => t.Name)
            .Take(request.Count)
            .ToListAsync(cancellationToken);

        return tags.ToTagDtos();
    }
}
