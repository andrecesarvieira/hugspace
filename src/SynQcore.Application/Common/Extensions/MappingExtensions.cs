using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Features.CorporateDocuments.DTOs;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Application.Features.DocumentTemplates.DTOs;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Application.DTOs;
using SynQcore.Application.DTOs.Notifications;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Domain.Entities;

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

    /// <summary>
    /// Mapeia FeedEntry para FeedItemDto
    /// </summary>
    public static FeedItemDto ToFeedItemDto(this FeedEntry feedEntry)
    {
        ArgumentNullException.ThrowIfNull(feedEntry);

        return new FeedItemDto
        {
            Id = feedEntry.Id,
            PostId = feedEntry.PostId,
            Priority = feedEntry.Priority.ToString(),
            RelevanceScore = feedEntry.RelevanceScore,
            Reason = feedEntry.Reason.ToString(),
            CreatedAt = feedEntry.CreatedAt,
            ViewedAt = feedEntry.ViewedAt,
            IsRead = feedEntry.IsRead,
            IsBookmarked = feedEntry.IsBookmarked,

            // Post information - será preenchido nos handlers se Post incluído
            Title = feedEntry.Post?.Title ?? string.Empty,
            Content = feedEntry.Post?.Content ?? string.Empty,
            Summary = feedEntry.Post?.Summary,
            PostType = feedEntry.Post?.Type.ToString() ?? string.Empty,
            ImageUrl = feedEntry.Post?.ImageUrl,
            IsPinned = feedEntry.Post?.IsPinned ?? false,
            IsOfficial = feedEntry.Post?.IsOfficial ?? false,

            // Author information - será preenchido se Author incluído
            AuthorId = feedEntry.Post?.AuthorId ?? Guid.Empty,
            AuthorName = feedEntry.Post?.Author != null
                ? $"{feedEntry.Post.Author.FirstName} {feedEntry.Post.Author.LastName}".Trim()
                : string.Empty,
            AuthorEmail = feedEntry.Post?.Author?.Email ?? string.Empty,
            AuthorAvatarUrl = feedEntry.Post?.Author?.ProfilePhotoUrl,
            AuthorDepartment = feedEntry.Post?.Department?.Name,

            // Engagement metrics - valores padrão
            LikeCount = 0,
            CommentCount = 0,
            ViewCount = 0,

            // User interaction - valores padrão
            HasLiked = false,
            HasCommented = false,

            // Tags and categories - será preenchido se incluídos
            Tags = feedEntry.Post?.PostTags?.Select(pt => pt.Tag.Name).ToList() ?? [],
            Category = feedEntry.Post?.Category?.Name
        };
    }

    /// <summary>
    /// Mapeia lista de FeedEntry para lista de FeedItemDto
    /// </summary>
    public static List<FeedItemDto> ToFeedItemDtos(this IEnumerable<FeedEntry> feedEntries)
    {
        return feedEntries.Select(fe => fe.ToFeedItemDto()).ToList();
    }

    /// <summary>
    /// Mapeia UserInterest para UserInterestDto
    /// </summary>
    public static UserInterestDto ToUserInterestDto(this UserInterest userInterest)
    {
        ArgumentNullException.ThrowIfNull(userInterest);

        return new UserInterestDto
        {
            Id = userInterest.Id,
            Type = userInterest.Type.ToString(),
            Value = userInterest.InterestValue,
            Score = userInterest.Score,
            InteractionCount = userInterest.InteractionCount,
            Source = userInterest.Source.ToString(),
            FirstInteractionAt = userInterest.CreatedAt, // Usando CreatedAt como proxy
            LastInteractionAt = userInterest.LastInteractionAt
        };
    }

    /// <summary>
    /// Mapeia lista de UserInterest para lista de UserInterestDto
    /// </summary>
    public static List<UserInterestDto> ToUserInterestDtos(this IEnumerable<UserInterest> userInterests)
    {
        return userInterests.Select(ui => ui.ToUserInterestDto()).ToList();
    }

    // ===== NOTIFICATION MAPPING EXTENSIONS =====

    /// <summary>
    /// Converte CorporateNotification entity para CorporateNotificationDto
    /// </summary>
    public static CorporateNotificationDto ToCorporateNotificationDto(this CorporateNotification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);

        return new CorporateNotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Content = notification.Content,
            Type = notification.Type.ToString(),
            Priority = notification.Priority.ToString(),
            Status = notification.Status.ToString(),
            CreatedBy = notification.CreatedByEmployee?.ToEmployeeBasicDto() ?? new EmployeeBasicDto(),
            TargetDepartment = notification.TargetDepartment?.ToDepartmentBasicDto(),
            CreatedAt = notification.CreatedAt,
            ExpiresAt = notification.ExpiresAt,
            ScheduledFor = notification.ScheduledFor,
            RequiresApproval = notification.RequiresApproval,
            ApprovedBy = notification.ApprovedByEmployee?.ToEmployeeBasicDto(),
            ApprovedAt = notification.ApprovedAt,
            RequiresAcknowledgment = notification.RequiresAcknowledgment,
            EnabledChannels = GetEnabledChannelsList(notification.EnabledChannels)
        };
    }

    /// <summary>
    /// Converte lista de CorporateNotification entities para lista de CorporateNotificationDto
    /// </summary>
    public static List<CorporateNotificationDto> ToCorporateNotificationDtos(this IEnumerable<CorporateNotification> notifications)
    {
        ArgumentNullException.ThrowIfNull(notifications);
        return notifications.Select(n => n.ToCorporateNotificationDto()).ToList();
    }

    /// <summary>
    /// Converte NotificationDelivery entity para NotificationDeliveryDto
    /// </summary>
    public static NotificationDeliveryDto ToNotificationDeliveryDto(this NotificationDelivery delivery)
    {
        ArgumentNullException.ThrowIfNull(delivery);

        return new NotificationDeliveryDto
        {
            Id = delivery.Id,
            NotificationId = delivery.NotificationId,
            Employee = delivery.Employee?.ToEmployeeBasicDto() ?? new EmployeeBasicDto(),
            Status = delivery.Status.ToString(),
            Channel = delivery.Channel.ToString(),
            DeliveredAt = delivery.DeliveredAt,
            ReadAt = delivery.ReadAt,
            AcknowledgedAt = delivery.AcknowledgedAt,
            DeliveryAttempts = delivery.DeliveryAttempts,
            ErrorDetails = delivery.ErrorDetails
        };
    }

    /// <summary>
    /// Converte lista de NotificationDelivery entities para lista de NotificationDeliveryDto
    /// </summary>
    public static List<NotificationDeliveryDto> ToNotificationDeliveryDtos(this IEnumerable<NotificationDelivery> deliveries)
    {
        ArgumentNullException.ThrowIfNull(deliveries);
        return deliveries.Select(d => d.ToNotificationDeliveryDto()).ToList();
    }

    /// <summary>
    /// Converte NotificationTemplate entity para NotificationTemplateDto
    /// </summary>
    public static NotificationTemplateDto ToNotificationTemplateDto(this NotificationTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return new NotificationTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Code = template.Code,
            Category = template.Category,
            TitleTemplate = template.TitleTemplate,
            ContentTemplate = template.ContentTemplate,
            DefaultType = template.DefaultType.ToString(),
            DefaultPriority = template.DefaultPriority.ToString(),
            DefaultChannels = GetEnabledChannelsList(template.DefaultChannels),
            DefaultRequiresApproval = template.DefaultRequiresApproval,
            DefaultRequiresAcknowledgment = template.DefaultRequiresAcknowledgment,
            IsActive = template.IsActive,
            AvailablePlaceholders = ParseAvailablePlaceholders(template.AvailablePlaceholders),
            CreatedAt = template.CreatedAt
        };
    }

    /// <summary>
    /// Converte lista de NotificationTemplate entities para lista de NotificationTemplateDto
    /// </summary>
    public static List<NotificationTemplateDto> ToNotificationTemplateDtos(this IEnumerable<NotificationTemplate> templates)
    {
        ArgumentNullException.ThrowIfNull(templates);
        return templates.Select(t => t.ToNotificationTemplateDto()).ToList();
    }

    /// <summary>
    /// Converte Employee entity para EmployeeBasicDto (para notificações)
    /// </summary>
    public static EmployeeBasicDto ToEmployeeBasicDto(this Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        return new EmployeeBasicDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Position = employee.Position
        };
    }

    /// <summary>
    /// Converte Department entity para DepartmentBasicDto (para notificações)
    /// </summary>
    public static DepartmentBasicDto ToDepartmentBasicDto(this Department department)
    {
        ArgumentNullException.ThrowIfNull(department);

        return new DepartmentBasicDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code
        };
    }

    /// <summary>
    /// Converte flags de canais para lista de strings
    /// </summary>
    private static List<string> GetEnabledChannelsList(NotificationChannels channels)
    {
        var channelList = new List<string>();

        if (channels.HasFlag(NotificationChannels.InApp))
            channelList.Add("InApp");
        if (channels.HasFlag(NotificationChannels.Email))
            channelList.Add("Email");
        if (channels.HasFlag(NotificationChannels.Push))
            channelList.Add("Push");
        if (channels.HasFlag(NotificationChannels.SMS))
            channelList.Add("SMS");
        if (channels.HasFlag(NotificationChannels.Webhook))
            channelList.Add("Webhook");
        if (channels.HasFlag(NotificationChannels.Teams))
            channelList.Add("Teams");
        if (channels.HasFlag(NotificationChannels.Slack))
            channelList.Add("Slack");

        return channelList;
    }

    /// <summary>
    /// Converte string de placeholders para lista
    /// </summary>
    private static List<string> ParseAvailablePlaceholders(string? placeholders)
    {
        if (string.IsNullOrEmpty(placeholders))
            return new List<string>();

        return placeholders.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(p => p.Trim())
                          .ToList();
    }

    // ===== CORPORATE DOCUMENT MAPPING EXTENSIONS =====

    /// <summary>
    /// Converte CorporateDocument entity para CorporateDocumentDto
    /// </summary>
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
            RequiresApproval = false,
            Version = document.Version,
            FileSizeBytes = document.FileSizeBytes,
            FileName = document.OriginalFileName,
            ContentType = document.ContentType,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            AuthorId = document.UploadedByEmployeeId,
            AuthorName = document.UploadedByEmployee?.FullName ?? string.Empty,
            DepartmentId = document.OwnerDepartmentId,
            DepartmentName = document.OwnerDepartment?.Name,
            ApprovedById = document.ApprovedByEmployeeId,
            ApprovedByName = document.ApprovedByEmployee?.FullName,
            ApprovedAt = document.ApprovedAt?.DateTime,
            Tags = !string.IsNullOrEmpty(document.Tags) ? document.Tags.Split(',').ToList() : new List<string>(),
            ViewCount = 0,
            DownloadCount = document.DownloadCount
        };
    }

    /// <summary>
    /// Converte CorporateDocument entity para CorporateDocumentDetailDto
    /// </summary>
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
            Version = document.Version,
            FileSizeBytes = document.FileSizeBytes,
            FileName = document.OriginalFileName,
            ContentType = document.ContentType,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            AuthorId = document.UploadedByEmployeeId,
            AuthorName = document.UploadedByEmployee?.FullName ?? string.Empty,
            DepartmentId = document.OwnerDepartmentId,
            DepartmentName = document.OwnerDepartment?.Name,
            ApprovedById = document.ApprovedByEmployeeId,
            ApprovedByName = document.ApprovedByEmployee?.FullName,
            ApprovedAt = document.ApprovedAt?.DateTime,
            Tags = !string.IsNullOrEmpty(document.Tags) ? document.Tags.Split(',').ToList() : new List<string>(),
            DownloadCount = document.DownloadCount
        };
    }

    /// <summary>
    /// Converte lista de CorporateDocument entities para lista de CorporateDocumentDto
    /// </summary>
    public static List<CorporateDocumentDto> ToCorporateDocumentDtos(this IEnumerable<CorporateDocument> documents)
    {
        return documents.Select(d => d.ToCorporateDocumentDto()).ToList();
    }

    // ===== MEDIA ASSET MAPPING EXTENSIONS =====

    /// <summary>
    /// Converte MediaAsset entity para MediaAssetDto
    /// </summary>
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
            CreatedByName = asset.UploadedByEmployee?.FullName ?? string.Empty,
            DepartmentId = null, // MediaAsset não tem DepartmentId direto
            DepartmentName = null,
            Tags = !string.IsNullOrEmpty(asset.Tags) ? asset.Tags.Split(',').ToList() : new List<string>(),
            ViewCount = 0, // Pode ser implementado depois
            DownloadCount = asset.DownloadCount
        };
    }

    /// <summary>
    /// Converte lista de MediaAsset entities para lista de MediaAssetDto
    /// </summary>
    public static List<MediaAssetDto> ToMediaAssetDtos(this IEnumerable<MediaAsset> assets)
    {
        return assets.Select(a => a.ToMediaAssetDto()).ToList();
    }

    // ===== DOCUMENT TEMPLATE MAPPING EXTENSIONS =====

    /// <summary>
    /// Converte DocumentTemplate entity para DocumentTemplateDto
    /// </summary>
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
            RequiresApproval = false, // Pode ser implementado depois
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            CreatedById = template.CreatedByEmployeeId,
            CreatedByName = template.CreatedByEmployee?.FullName ?? string.Empty,
            AllowedDepartments = new List<string>(), // Pode ser implementado depois
            UsageCount = template.UsageCount,
            LastUsedAt = template.LastUsedAt?.DateTime
        };
    }

    /// <summary>
    /// Converte lista de DocumentTemplate entities para lista de DocumentTemplateDto
    /// </summary>
    public static List<DocumentTemplateDto> ToDocumentTemplateDtos(this IEnumerable<DocumentTemplate> templates)
    {
        return templates.Select(t => t.ToDocumentTemplateDto()).ToList();
    }
}
