using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.DTOs;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.CorporateDocuments.DTOs;
using SynQcore.Application.Features.DocumentTemplates.DTOs;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Domain.Entities;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Common.Extensions;

public static class MappingExtensions
{
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

    public static List<EmployeeDto> ToEmployeeDtos(this IEnumerable<Employee> employees)
    {
        return employees.Select(e => e.ToEmployeeDto()).ToList();
    }

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

    public static List<EndorsementDto> ToEndorsementDtos(this IEnumerable<Endorsement> endorsements)
    {
        return endorsements.Select(e => e.ToEndorsementDto()).ToList();
    }

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

    public static List<TagDto> ToTagDtos(this IEnumerable<Tag> tags)
    {
        return tags.Select(t => t.ToTagDto()).ToList();
    }

    // ===== KNOWLEDGE CATEGORY MAPPING EXTENSIONS =====

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

    public static List<KnowledgeCategoryDto> ToKnowledgeCategoryDtos(this IEnumerable<KnowledgeCategory> categories)
    {
        return categories.Select(c => c.ToKnowledgeCategoryDto()).ToList();
    }

    // ===== KNOWLEDGE POST MAPPING EXTENSIONS =====

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

    public static CorporateDocumentDto ToCorporateDocumentDto(this CorporateDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new CorporateDocumentDto
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            Category = document.Category.ToString(),
            Status = document.Status,
            AccessLevel = document.AccessLevel,
            RequiresApproval = document.Status == DocumentStatus.Draft,
            Version = document.Version,
            FileSizeBytes = document.FileSizeBytes,
            FileName = document.OriginalFileName,
            ContentType = document.ContentType,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            AuthorId = document.UploadedByEmployeeId,
            AuthorName = string.Empty, // Será preenchido no handler se necessário
            DepartmentId = document.OwnerDepartmentId,
            DepartmentName = string.Empty, // Será preenchido no handler se necessário
            ApprovedById = document.ApprovedByEmployeeId,
            ApprovedByName = string.Empty // Será preenchido no handler se necessário
        };
    }

    public static List<CorporateDocumentDto> ToCorporateDocumentDtos(this IEnumerable<CorporateDocument> documents)
    {
        ArgumentNullException.ThrowIfNull(documents);
        return documents.Select(d => d.ToCorporateDocumentDto()).ToList();
    }

    public static CorporateDocumentDetailDto ToCorporateDocumentDetailDto(this CorporateDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new CorporateDocumentDetailDto
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            Category = document.Category.ToString(),
            Status = document.Status,
            AccessLevel = document.AccessLevel,
            RequiresApproval = document.Status == DocumentStatus.Draft,
            Version = document.Version,
            FileSizeBytes = document.FileSizeBytes,
            FileName = document.OriginalFileName,
            ContentType = document.ContentType,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            AuthorId = document.UploadedByEmployeeId,
            AuthorName = string.Empty, // Será preenchido no handler se necessário
            DepartmentId = document.OwnerDepartmentId,
            DepartmentName = string.Empty, // Será preenchido no handler se necessário
            ApprovedById = document.ApprovedByEmployeeId,
            ApprovedByName = string.Empty, // Será preenchido no handler se necessário
            ApprovalNotes = null, // Será preenchido no handler se necessário
            RejectionReason = null, // Será preenchido no handler se necessário
            LastAccessedAt = null, // Será preenchido no handler se necessário
            Versions = [], // Será preenchido no handler se necessário
            RecentAccesses = [] // Será preenchido no handler se necessário
        };
    }

    // ===== MEDIA ASSETS MAPPING EXTENSIONS =====

    public static MediaAssetDto ToMediaAssetDto(this MediaAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        return new MediaAssetDto
        {
            Id = asset.Id,
            Title = asset.Name,
            Description = asset.Description,
            AssetType = asset.Type.ToString(),
            AccessLevel = asset.AccessLevel,
            FileSizeBytes = asset.FileSizeBytes,
            FileName = asset.OriginalFileName,
            ContentType = asset.ContentType,
            ThumbnailPath = asset.ThumbnailUrl,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt,
            Width = asset.Width,
            Height = asset.Height,
            Duration = asset.DurationSeconds,
            CreatedById = asset.UploadedByEmployeeId,
            CreatedByName = string.Empty, // Será preenchido no handler se necessário
            DepartmentId = null, // MediaAsset não tem DepartmentId direto
            DepartmentName = string.Empty, // Será preenchido no handler se necessário
            Tags = [] // Será preenchido no handler se necessário
        };
    }

    public static List<MediaAssetDto> ToMediaAssetDtos(this IEnumerable<MediaAsset> assets)
    {
        ArgumentNullException.ThrowIfNull(assets);
        return assets.Select(a => a.ToMediaAssetDto()).ToList();
    }

    public static MediaAssetDetailDto ToMediaAssetDetailDto(this MediaAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        var dto = asset.ToMediaAssetDto();
        return new MediaAssetDetailDto
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            AssetType = dto.AssetType,
            AccessLevel = dto.AccessLevel,
            FileSizeBytes = dto.FileSizeBytes,
            FileName = dto.FileName,
            ContentType = dto.ContentType,
            ThumbnailPath = dto.ThumbnailPath,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Width = dto.Width,
            Height = dto.Height,
            Duration = dto.Duration,
            CreatedById = dto.CreatedById,
            CreatedByName = dto.CreatedByName,
            DepartmentId = dto.DepartmentId,
            DepartmentName = dto.DepartmentName,
            Tags = dto.Tags,
            ViewCount = dto.ViewCount,
            DownloadCount = dto.DownloadCount,
            RecentAccesses = [],
            Metadata = new Dictionary<string, string>
            {
                ["Type"] = asset.Type.ToString(),
                ["Category"] = asset.Category.ToString()
            }
        };
    }

    public static MediaAssetFileDto ToMediaAssetFileDto(this MediaAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        return new MediaAssetFileDto
        {
            FileData = [], // Em uma implementação real, carregaria do storage
            FileName = asset.OriginalFileName,
            ContentType = asset.ContentType
        };
    }

    public static MediaAssetThumbnailDto ToMediaAssetThumbnailDto(this MediaAsset asset, string size)
    {
        ArgumentNullException.ThrowIfNull(asset);

        return new MediaAssetThumbnailDto
        {
            FileData = [], // Em uma implementação real, carregaria o thumbnail
            ContentType = "image/jpeg"
        };
    }

    public static MediaAssetStatsDto ToMediaAssetStatsDto(this MediaAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        return new MediaAssetStatsDto
        {
            AssetId = asset.Id,
            TotalViews = 0, // Em uma implementação real, viria de log de acessos
            TotalDownloads = asset.DownloadCount,
            UniqueViewers = 0,
            UniqueDownloaders = 0,
            MostActiveUser = null,
            ViewsByDepartment = new Dictionary<string, int>(),
            ViewsByDate = new Dictionary<DateTime, int>()
        };
    }

    // ===== DOCUMENT TEMPLATE MAPPING EXTENSIONS =====

    public static DocumentTemplateDto ToDocumentTemplateDto(this DocumentTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return new DocumentTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            Category = template.DefaultCategory.ToString(),
            DefaultAccessLevel = template.DefaultAccessLevel,
            RequiresApproval = false, // Será preenchido no handler se necessário
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            CreatedById = template.CreatedByEmployeeId,
            CreatedByName = string.Empty, // Será preenchido no handler se necessário
            AllowedDepartments = [], // Será preenchido no handler se necessário
            UsageCount = 0, // Será preenchido no handler se necessário
            LastUsedAt = null // Será preenchido no handler se necessário
        };
    }

    public static List<DocumentTemplateDto> ToDocumentTemplateDtos(this IEnumerable<DocumentTemplate> templates)
    {
        ArgumentNullException.ThrowIfNull(templates);
        return templates.Select(t => t.ToDocumentTemplateDto()).ToList();
    }

    public static DocumentTemplateDetailDto ToDocumentTemplateDetailDto(this DocumentTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);

        var dto = template.ToDocumentTemplateDto();
        return new DocumentTemplateDetailDto
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            DefaultAccessLevel = dto.DefaultAccessLevel,
            RequiresApproval = dto.RequiresApproval,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedById = dto.CreatedById,
            CreatedByName = dto.CreatedByName,
            AllowedDepartments = dto.AllowedDepartments,
            UsageCount = template.UsageCount,
            LastUsedAt = template.LastUsedAt?.DateTime,
            Content = "", // Em uma implementação real, carregaria do storage
            Fields = [] // Será preenchido no handler se necessário
        };
    }

    // ===== USER INTEREST MAPPING EXTENSIONS =====

    public static UserInterestDto ToUserInterestDto(this UserInterest interest)
    {
        ArgumentNullException.ThrowIfNull(interest);

        return new UserInterestDto
        {
            Id = interest.Id,
            Type = interest.Type.ToString(),
            Value = interest.InterestValue,
            Score = interest.Score,
            InteractionCount = interest.InteractionCount,
            Source = interest.Source.ToString(),
            FirstInteractionAt = interest.CreatedAt, // UserInterest não tem FirstInteractionAt, usar CreatedAt
            LastInteractionAt = interest.LastInteractionAt
        };
    }

    public static List<UserInterestDto> ToUserInterestDtos(this IEnumerable<UserInterest> interests)
    {
        ArgumentNullException.ThrowIfNull(interests);
        return interests.Select(i => i.ToUserInterestDto()).ToList();
    }

    // ===== FEED MAPPING EXTENSIONS =====

    public static FeedItemDto ToFeedItemDto(this FeedEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return new FeedItemDto
        {
            Id = entry.Id,
            PostId = entry.PostId,
            Priority = entry.Priority.ToString(),
            RelevanceScore = entry.RelevanceScore,
            Reason = entry.Reason.ToString(),
            CreatedAt = entry.CreatedAt,
            ViewedAt = entry.ViewedAt,
            IsRead = entry.IsRead,
            IsBookmarked = entry.IsBookmarked,
            // Post information - será preenchido no handler
            Title = string.Empty,
            Content = string.Empty,
            Summary = null,
            PostType = string.Empty,
            ImageUrl = null,
            IsPinned = false,
            IsOfficial = false,
            // Author information - será preenchido no handler
            AuthorId = Guid.Empty,
            AuthorName = string.Empty,
            AuthorEmail = string.Empty,
            AuthorAvatarUrl = null,
            AuthorDepartment = null,
            // Engagement metrics - será preenchido no handler
            LikeCount = 0,
            CommentCount = 0,
            ViewCount = 0,
            // User interaction - será preenchido no handler
            HasLiked = false,
            HasCommented = false,
            // Tags and categories - será preenchido no handler
            Tags = [],
            Category = null
        };
    }

    public static List<FeedItemDto> ToFeedItemDtos(this IEnumerable<FeedEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        return entries.Select(e => e.ToFeedItemDto()).ToList();
    }

    public static async Task<PagedResult<DocumentTemplateDto>> ToPaginatedResultAsync(
        this IQueryable<DocumentTemplate> query, int page, int pageSize, CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<DocumentTemplateDto>
        {
            Items = items.Select(i => i.ToDocumentTemplateDto()).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public static async Task<PagedResult<MediaAssetDto>> ToPaginatedResultAsync(
        this IQueryable<MediaAsset> query, int page, int pageSize, CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<MediaAssetDto>
        {
            Items = items.Select(i => i.ToMediaAssetDto()).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    /// <summary>
    /// Extensão genérica para paginação com mapeamento customizado
    /// </summary>
    public static async Task<PagedResult<TDto>> ToPaginatedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        int page,
        int pageSize,
        Func<TEntity, TDto> mapper,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TDto>
        {
            Items = items.Select(mapper).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
}
