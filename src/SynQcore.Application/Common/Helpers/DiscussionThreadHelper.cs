using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Common.Helpers;

/// <summary>
/// Helper para gerenciamento de discussion threads corporativas
/// </summary>
public class DiscussionThreadHelper
{
    private readonly ISynQcoreDbContext _context;

    public DiscussionThreadHelper(ISynQcoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Calcula o nível e caminho da thread para um novo comentário
    /// </summary>
    public async Task<(int ThreadLevel, string ThreadPath)> CalculateThreadPositionAsync(
        Guid? parentCommentId,
        CancellationToken cancellationToken = default)
    {
        if (!parentCommentId.HasValue)
        {
            // Root comment
            return (0, "0");
        }

        var parentComment = await _context.Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == parentCommentId.Value, cancellationToken);

        if (parentComment == null)
        {
            throw new InvalidOperationException("Comentário pai não encontrado.");
        }

        var newLevel = parentComment.ThreadLevel + 1;
        
        // Conta quantos replies já existem para o comentário pai
        var siblingCount = await _context.Comments
            .Where(c => c.ParentCommentId == parentCommentId.Value)
            .CountAsync(cancellationToken);

        var newPath = $"{parentComment.ThreadPath}.{siblingCount + 1}";

        return (newLevel, newPath);
    }

    /// <summary>
    /// Atualiza contadores de replies em cascade
    /// </summary>
    public async Task UpdateReplyCountsAsync(Guid commentId, int delta, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);

        if (comment == null) return;

        // Atualiza o contador do comentário
        comment.ReplyCount += delta;

        // Se tem pai, atualiza recursivamente
        if (comment.ParentCommentId.HasValue)
        {
            await UpdateReplyCountsAsync(comment.ParentCommentId.Value, delta, cancellationToken);
        }

        // Atualiza LastActivityAt do post
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == comment.PostId, cancellationToken);

        if (post != null)
        {
            post.LastActivityAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Processa menções em um comentário
    /// </summary>
    public static List<CommentMention> ExtractMentions(string content, Guid commentId, Guid authorId)
    {
        var mentions = new List<CommentMention>();
        var mentionPattern = @"@(\w+(?:\.\w+)?)"; // Matches @username or @first.last

        var matches = System.Text.RegularExpressions.Regex.Matches(content, mentionPattern);

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var mentionText = match.Value;
            var startPosition = match.Index;
            var length = match.Length;

            // Aqui você poderia resolver o username para Employee ID
            // Por simplicidade, vou assumir que o frontend já passou os IDs corretos
            
            mentions.Add(new CommentMention
            {
                Id = Guid.NewGuid(),
                CommentId = commentId,
                MentionedById = authorId,
                MentionText = mentionText,
                StartPosition = startPosition,
                Length = length,
                Context = MentionContext.General,
                Urgency = MentionUrgency.Normal,
                CreatedAt = DateTime.UtcNow
            });
        }

        return mentions;
    }

    /// <summary>
    /// Valida se o usuário pode moderar o comentário
    /// </summary>
    public async Task<bool> CanUserModerateCommentAsync(Guid userId, Guid commentId, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments
            .Include(c => c.Author)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);

        if (comment == null) return false;

        var user = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);

        if (user == null) return false;

        // Autor pode moderar próprio comentário
        if (comment.AuthorId == userId) return true;

        // Managers podem moderar comentários de seus subordinados
        var isManager = await _context.Employees
            .AnyAsync(e => e.ManagerId == userId, cancellationToken);

        if (isManager)
        {
            var canModerate = await _context.Employees
                .Where(e => e.Id == comment.AuthorId)
                .AnyAsync(e => e.ManagerId == userId, cancellationToken);

            if (canModerate) return true;
        }

        // HR e Admin podem moderar qualquer comentário
        // Esta validação seria feita através de roles, mas aqui simplificando
        return false;
    }

    /// <summary>
    /// Calcula métricas de engagement para um comentário
    /// </summary>
    public async Task<(int LikeCount, int EndorsementCount)> CalculateEngagementMetricsAsync(
        Guid commentId, 
        CancellationToken cancellationToken = default)
    {
        var likeCount = await _context.CommentLikes
            .CountAsync(cl => cl.CommentId == commentId, cancellationToken);

        var endorsementCount = await _context.Endorsements
            .CountAsync(e => e.CommentId == commentId, cancellationToken);

        return (likeCount, endorsementCount);
    }
}