using MediatR;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Application.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace SynQcore.Application.Features.KnowledgeManagement.Commands;

// Command para criar novo artigo de conhecimento
public class CreateKnowledgePostCommand : IRequest<KnowledgePostDto>
{
    public CreateKnowledgePostDto Data { get; set; } = null!;
    public Guid AuthorId { get; set; }
}

public class CreateKnowledgePostCommandHandler : IRequestHandler<CreateKnowledgePostCommand, KnowledgePostDto>
{
    private readonly ISynQcoreDbContext _context;

    public CreateKnowledgePostCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<KnowledgePostDto> Handle(CreateKnowledgePostCommand request, CancellationToken cancellationToken)
    {
        // Verificar se autor existe
        var authorExists = await _context.Employees
            .AnyAsync(e => e.Id == request.AuthorId && !e.IsDeleted, cancellationToken);

        if (!authorExists)
            throw new NotFoundException("Autor não encontrado.");

        // Verificar se categoria existe (se especificada)
        if (request.Data.CategoryId.HasValue)
        {
            var categoryExists = await _context.KnowledgeCategories
                .AnyAsync(c => c.Id == request.Data.CategoryId && !c.IsDeleted, cancellationToken);

            if (!categoryExists)
                throw new NotFoundException("Categoria não encontrada.");
        }

        // Verificar se post pai existe (se especificado)
        if (request.Data.ParentPostId.HasValue)
        {
            var parentExists = await _context.Posts
                .AnyAsync(p => p.Id == request.Data.ParentPostId && !p.IsDeleted, cancellationToken);

            if (!parentExists)
                throw new NotFoundException("Post pai não encontrado.");
        }

        var post = new Post
        {
            Title = request.Data.Title,
            Content = request.Data.Content,
            Summary = request.Data.Summary,
            ImageUrl = request.Data.ImageUrl,
            DocumentUrl = request.Data.DocumentUrl,
            Type = request.Data.Type,
            Status = request.Data.Status,
            Visibility = request.Data.Visibility,
            RequiresApproval = request.Data.RequiresApproval,
            CategoryId = request.Data.CategoryId,
            DepartmentId = request.Data.DepartmentId,
            TeamId = request.Data.TeamId,
            ParentPostId = request.Data.ParentPostId,
            AuthorId = request.AuthorId,
            Version = "1.0"
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

        // Associar tags se fornecidas
        if (request.Data.TagIds.Count > 0)
        {
            var validTagIds = await _context.Tags
                .Where(t => request.Data.TagIds.Contains(t.Id) && !t.IsDeleted)
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);

            foreach (var tagId in validTagIds)
            {
                var postTag = new PostTag
                {
                    PostId = post.Id,
                    TagId = tagId
                };
                _context.PostTags.Add(postTag);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        // Recarregar com relacionamentos para retornar
        var createdPost = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Department)
            .Include(p => p.Team)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .FirstAsync(p => p.Id == post.Id, cancellationToken);

        return createdPost.ToKnowledgePostDto();
    }
}

// Command para atualizar artigo de conhecimento
public class UpdateKnowledgePostCommand : IRequest<KnowledgePostDto>
{
    public Guid Id { get; set; }
    public UpdateKnowledgePostDto Data { get; set; } = null!;
    public Guid UserId { get; set; }
}

public class UpdateKnowledgePostCommandHandler : IRequestHandler<UpdateKnowledgePostCommand, KnowledgePostDto>
{
    private readonly ISynQcoreDbContext _context;

    public UpdateKnowledgePostCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task<KnowledgePostDto> Handle(UpdateKnowledgePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (post == null)
            throw new NotFoundException($"Artigo com ID {request.Id} não encontrado.");

        // Verificar se usuário pode editar (autor ou admin)
        if (post.AuthorId != request.UserId)
        {
            // Aqui você pode adicionar lógica para verificar se é admin/HR
            // Por enquanto, apenas o autor pode editar
            throw new UnauthorizedAccessException("Apenas o autor pode editar este artigo.");
        }

        // Atualizar propriedades se fornecidas
        if (!string.IsNullOrEmpty(request.Data.Title))
            post.Title = request.Data.Title;

        if (!string.IsNullOrEmpty(request.Data.Content))
            post.Content = request.Data.Content;

        if (request.Data.Summary != null)
            post.Summary = request.Data.Summary;

        if (request.Data.ImageUrl != null)
            post.ImageUrl = request.Data.ImageUrl;

        if (request.Data.DocumentUrl != null)
            post.DocumentUrl = request.Data.DocumentUrl;

        if (request.Data.Type.HasValue)
            post.Type = request.Data.Type.Value;

        if (request.Data.Status.HasValue)
            post.Status = request.Data.Status.Value;

        if (request.Data.Visibility.HasValue)
            post.Visibility = request.Data.Visibility.Value;

        if (request.Data.RequiresApproval.HasValue)
            post.RequiresApproval = request.Data.RequiresApproval.Value;

        if (request.Data.CategoryId != post.CategoryId)
        {
            if (request.Data.CategoryId.HasValue)
            {
                var categoryExists = await _context.KnowledgeCategories
                    .AnyAsync(c => c.Id == request.Data.CategoryId && !c.IsDeleted, cancellationToken);

                if (!categoryExists)
                    throw new NotFoundException("Categoria não encontrada.");
            }
            post.CategoryId = request.Data.CategoryId;
        }

        if (request.Data.DepartmentId != post.DepartmentId)
            post.DepartmentId = request.Data.DepartmentId;

        if (request.Data.TeamId != post.TeamId)
            post.TeamId = request.Data.TeamId;

        // Atualizar tags se fornecidas
        if (request.Data.TagIds != null)
        {
            // Remover tags existentes
            var existingPostTags = post.PostTags.ToList();
            foreach (var postTag in existingPostTags)
            {
                _context.PostTags.Remove(postTag);
            }

            // Adicionar novas tags
            if (request.Data.TagIds.Count > 0)
            {
                var validTagIds = await _context.Tags
                    .Where(t => request.Data.TagIds.Contains(t.Id) && !t.IsDeleted)
                    .Select(t => t.Id)
                    .ToListAsync(cancellationToken);

                foreach (var tagId in validTagIds)
                {
                    var postTag = new PostTag
                    {
                        PostId = post.Id,
                        TagId = tagId
                    };
                    _context.PostTags.Add(postTag);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Recarregar com relacionamentos
        var updatedPost = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Department)
            .Include(p => p.Team)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .FirstAsync(p => p.Id == post.Id, cancellationToken);

        return updatedPost.ToKnowledgePostDto();
    }
}

// Command para excluir artigo de conhecimento
public class DeleteKnowledgePostCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

public class DeleteKnowledgePostCommandHandler : IRequestHandler<DeleteKnowledgePostCommand>
{
    private readonly ISynQcoreDbContext _context;

    public DeleteKnowledgePostCommandHandler(ISynQcoreDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteKnowledgePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (post == null)
            throw new NotFoundException($"Artigo com ID {request.Id} não encontrado.");

        // Verificar se usuário pode excluir (autor ou admin)
        if (post.AuthorId != request.UserId)
        {
            // Aqui você pode adicionar lógica para verificar se é admin/HR
            throw new UnauthorizedAccessException("Apenas o autor pode excluir este artigo.");
        }

        // Verificar se tem versões dependentes
        if (post.Versions.Count > 0)
            throw new ValidationException("Não é possível excluir artigo que possui versões dependentes.");

        post.IsDeleted = true;
        post.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}