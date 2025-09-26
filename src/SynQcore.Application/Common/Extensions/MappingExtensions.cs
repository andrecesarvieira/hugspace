using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Common.Extensions;

/// <summary>
/// Extensions para mapeamento manual de entidades para DTOs - substituição do AutoMapper
/// Implementação performática sem dependências externas
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Mapeia Employee para EmployeeDto
    /// </summary>
    public static EmployeeDto ToEmployeeDto(this Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        
        return new EmployeeDto
        {
            Id = employee.Id,
            Email = employee.Email,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Phone = employee.Phone,
            Avatar = employee.ProfilePhotoUrl,
            HireDate = employee.HireDate,
            IsActive = employee.IsActive,
            ManagerId = employee.ManagerId,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            // Collections vazias por padrão - serão preenchidas nos handlers específicos se necessário
            Departments = [],
            Teams = []
        };
    }

    /// <summary>
    /// Mapeia lista de Employee para lista de EmployeeDto
    /// </summary>
    public static List<EmployeeDto> ToEmployeeDtos(this IEnumerable<Employee> employees)
    {
        return employees.Select(e => e.ToEmployeeDto()).ToList();
    }

    /// <summary>
    /// Mapeia Comment para DiscussionCommentDto
    /// </summary>
    public static DiscussionCommentDto ToDiscussionCommentDto(this Comment? comment)
    {
        ArgumentNullException.ThrowIfNull(comment);

        return new DiscussionCommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            ParentCommentId = comment.ParentCommentId,
            PostId = comment.PostId,
            AuthorId = comment.AuthorId,
            AuthorName = comment.Author?.FullName ?? string.Empty,
            AuthorJobTitle = comment.Author?.JobTitle ?? string.Empty,
            AuthorProfilePhotoUrl = comment.Author?.ProfilePhotoUrl,
            Type = comment.Type.ToString(),
            IsResolved = comment.IsResolved,
            ResolvedByName = comment.ResolvedBy?.FullName,
            ResolvedAt = comment.ResolvedAt,
            ResolutionNote = comment.ResolutionNote,
            IsEdited = comment.IsEdited,
            EditedAt = comment.EditedAt,
            IsFlagged = comment.IsFlagged,
            ModerationStatus = comment.ModerationStatus.ToString(),
            ModeratedAt = comment.ModeratedAt,
            ModerationReason = comment.ModerationReason,
            Visibility = comment.Visibility.ToString(),
            IsConfidential = comment.IsConfidential,
            ThreadLevel = comment.ThreadLevel,
            ThreadPath = comment.ThreadPath,
            IsHighlighted = comment.IsHighlighted,
            Priority = comment.Priority.ToString(),
            LikeCount = comment.LikeCount,
            ReplyCount = comment.ReplyCount,
            EndorsementCount = comment.EndorsementCount,
            LastActivityAt = comment.LastActivityAt,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            // Collections
            Replies = comment.Replies?.Select(r => r.ToDiscussionCommentDto()).ToList() ?? [],
            Mentions = comment.Mentions?.Select(m => m.ToCommentMentionDto()).ToList() ?? []
        };
    }

    /// <summary>
    /// Mapeia CommentMention para CommentMentionDto
    /// </summary>
    public static CommentMentionDto ToCommentMentionDto(this CommentMention mention)
    {
        return new CommentMentionDto
        {
            Id = mention.Id,
            CommentId = mention.CommentId,
            MentionedEmployeeId = mention.MentionedEmployeeId,
            MentionedEmployeeName = mention.MentionedEmployee?.FullName ?? string.Empty,
            MentionText = mention.MentionText,
            StartPosition = mention.StartPosition,
            Length = mention.Length,
            HasBeenNotified = mention.HasBeenNotified,
            NotifiedAt = mention.NotifiedAt,
            IsRead = mention.IsRead,
            ReadAt = mention.ReadAt,
            Context = mention.Context.ToString(),
            Urgency = mention.Urgency.ToString(),
            CreatedAt = mention.CreatedAt
        };
    }

    /// <summary>
    /// Mapeia Endorsement para EndorsementDto
    /// </summary>
    public static EndorsementDto ToEndorsementDto(this Endorsement endorsement)
    {
        ArgumentNullException.ThrowIfNull(endorsement);
        
        return new EndorsementDto
        {
            Id = endorsement.Id,
            Type = endorsement.Type,
            TypeDisplayName = GetEndorsementTypeDisplayName(endorsement.Type),
            TypeIcon = GetEndorsementTypeIcon(endorsement.Type),
            Note = endorsement.Note,
            IsPublic = endorsement.IsPublic,
            EndorsedAt = endorsement.EndorsedAt,
            Context = endorsement.Context,
            EndorserId = endorsement.EndorserId,
            EndorserName = endorsement.Endorser?.FullName ?? string.Empty,
            EndorserEmail = endorsement.Endorser?.Email ?? string.Empty,
            EndorserDepartment = null, // Será preenchido se necessário
            EndorserPosition = endorsement.Endorser?.Position,
            PostId = endorsement.PostId,
            PostTitle = endorsement.Post?.Title,
            CommentId = endorsement.CommentId,
            CommentContent = endorsement.Comment?.Content
        };
    }

    /// <summary>
    /// Mapeia lista de Endorsement para lista de EndorsementDto
    /// </summary>
    public static List<EndorsementDto> ToEndorsementDtos(this IEnumerable<Endorsement> endorsements)
    {
        return endorsements.Select(e => e.ToEndorsementDto()).ToList();
    }

    /// <summary>
    /// Obtém nome de exibição do tipo de endorsement
    /// </summary>
    private static string GetEndorsementTypeDisplayName(EndorsementType type)
    {
        return type switch
        {
            EndorsementType.Helpful => "Útil",
            EndorsementType.Insightful => "Perspicaz",
            EndorsementType.Accurate => "Preciso",
            EndorsementType.Innovative => "Inovador",
            EndorsementType.Comprehensive => "Abrangente",
            EndorsementType.WellResearched => "Bem Pesquisado",
            EndorsementType.Actionable => "Aplicável",
            EndorsementType.Strategic => "Estratégico",
            _ => type.ToString()
        };
    }

    /// <summary>
    /// Obtém ícone do tipo de endorsement
    /// </summary>
    private static string GetEndorsementTypeIcon(EndorsementType type)
    {
        return type switch
        {
            EndorsementType.Helpful => "🔥",
            EndorsementType.Insightful => "💡",
            EndorsementType.Accurate => "✅",
            EndorsementType.Innovative => "🚀",
            EndorsementType.Comprehensive => "📚",
            EndorsementType.WellResearched => "🔍",
            EndorsementType.Actionable => "⚡",
            EndorsementType.Strategic => "🎯",
            _ => "👍"
        };
    }

    /// <summary>
    /// Mapeia dados para EmployeeEndorsementRankingDto
    /// </summary>
    public static EmployeeEndorsementRankingDto ToEmployeeEndorsementRankingDto(
        this Employee employee,
        int totalReceived,
        int totalGiven,
        int helpfulReceived,
        int insightfulReceived,
        int accurateReceived,
        int innovativeReceived,
        double engagementScore,
        int ranking = 0)
    {
        return new EmployeeEndorsementRankingDto
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.FullName,
            EmployeeEmail = employee.Email,
            Department = null, // Será preenchido pelo handler se necessário
            Position = employee.Position,
            TotalEndorsementsReceived = totalReceived,
            TotalEndorsementsGiven = totalGiven,
            HelpfulReceived = helpfulReceived,
            InsightfulReceived = insightfulReceived,
            AccurateReceived = accurateReceived,
            InnovativeReceived = innovativeReceived,
            EngagementScore = engagementScore,
            Ranking = ranking
        };
    }

    // ===== TAG MAPPING EXTENSIONS =====

    /// <summary>
    /// Converte Tag entity para TagDto
    /// </summary>
    public static TagDto ToTagDto(this Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            Type = tag.Type,
            Color = tag.Color,
            UsageCount = tag.UsageCount,
            CreatedAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt
        };
    }

    /// <summary>
    /// Converte lista de Tag entities para lista de TagDto
    /// </summary>
    public static List<TagDto> ToTagDtos(this IEnumerable<Tag> tags)
    {
        return tags.Select(t => t.ToTagDto()).ToList();
    }

    // ===== KNOWLEDGE CATEGORY MAPPING EXTENSIONS =====

    /// <summary>
    /// Converte KnowledgeCategory entity para KnowledgeCategoryDto
    /// </summary>
    public static KnowledgeCategoryDto ToKnowledgeCategoryDto(this KnowledgeCategory category)
    {
        return new KnowledgeCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            Icon = category.Icon,
            ParentCategoryId = category.ParentCategoryId,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    /// <summary>
    /// Converte lista de KnowledgeCategory entities para lista de KnowledgeCategoryDto
    /// </summary>
    public static List<KnowledgeCategoryDto> ToKnowledgeCategoryDtos(this IEnumerable<KnowledgeCategory> categories)
    {
        return categories.Select(c => c.ToKnowledgeCategoryDto()).ToList();
    }

    // ===== KNOWLEDGE POST MAPPING EXTENSIONS =====

    /// <summary>
    /// Converte Post entity para KnowledgePostDto
    /// </summary>
    public static KnowledgePostDto ToKnowledgePostDto(this Post post)
    {
        ArgumentNullException.ThrowIfNull(post);
        
        return new KnowledgePostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Summary = post.Summary,
            ImageUrl = post.ImageUrl,
            DocumentUrl = post.DocumentUrl,
            Type = post.Type,
            Status = post.Status,
            Visibility = post.Visibility,
            Version = post.Version,
            RequiresApproval = post.RequiresApproval,
            ViewCount = post.ViewCount,
            LikeCount = post.LikeCount,
            CommentCount = post.CommentCount,
            AuthorId = post.AuthorId,
            AuthorName = post.Author?.FullName ?? string.Empty,
            AuthorEmail = post.Author?.Email ?? string.Empty,
            CategoryId = post.CategoryId,
            CategoryName = post.Category?.Name ?? string.Empty,
            DepartmentId = post.DepartmentId,
            DepartmentName = post.Department?.Name ?? string.Empty,
            TeamId = post.TeamId,
            TeamName = post.Team?.Name ?? string.Empty,
            ParentPostId = post.ParentPostId,
            ParentPostTitle = post.ParentPost?.Title,
            Tags = post.PostTags?.Select(pt => pt.Tag.ToTagDto()).ToList() ?? [],
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}