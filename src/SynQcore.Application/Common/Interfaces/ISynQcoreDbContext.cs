using Microsoft.EntityFrameworkCore;
using SynQcore.Domain.Entities;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Domain.Entities.Relationships;

namespace SynQcore.Application.Common.Interfaces;

public interface ISynQcoreDbContext
{
    // Organization entities
    DbSet<Employee> Employees { get; }
    DbSet<Department> Departments { get; }
    DbSet<Team> Teams { get; }
    DbSet<Position> Positions { get; }

    // Communication entities
    DbSet<Post> Posts { get; }
    DbSet<Comment> Comments { get; }
    DbSet<CommentLike> CommentLikes { get; }
    DbSet<CommentMention> CommentMentions { get; }
    DbSet<PostLike> PostLikes { get; }
    DbSet<Notification> Notifications { get; }

    // Feed and Discovery entities (Fase 3.4)
    DbSet<FeedEntry> FeedEntries { get; }
    DbSet<UserInterest> UserInterests { get; }

    // Knowledge Management entities
    DbSet<KnowledgeCategory> KnowledgeCategories { get; }
    DbSet<Tag> Tags { get; }
    DbSet<PostTag> PostTags { get; }
    DbSet<Endorsement> Endorsements { get; }

    // Corporate Notifications entities (Fase 4.2)
    DbSet<CorporateNotification> CorporateNotifications { get; }
    DbSet<NotificationDelivery> NotificationDeliveries { get; }
    DbSet<NotificationTemplate> NotificationTemplates { get; }

    // Corporate Document Management entities (Fase 4.3)
    DbSet<CorporateDocument> CorporateDocuments { get; }
    DbSet<DocumentAccess> DocumentAccesses { get; }
    DbSet<DocumentAccessLog> DocumentAccessLogs { get; }
    DbSet<DocumentTemplate> DocumentTemplates { get; }
    DbSet<MediaAsset> MediaAssets { get; }

    // Security & Audit entities (Fase 6)
    DbSet<AuditLogEntity> AuditLogs { get; }

    // Privacy & LGPD Compliance entities (Fase 5)
    DbSet<ConsentRecord> ConsentRecords { get; }
    DbSet<DataExportRequest> DataExportRequests { get; }
    DbSet<DataDeletionRequest> DataDeletionRequests { get; }
    DbSet<PersonalDataCategory> PersonalDataCategories { get; }

    // Relationship entities
    DbSet<EmployeeDepartment> EmployeeDepartments { get; }
    DbSet<TeamMembership> TeamMemberships { get; }
    DbSet<ReportingRelationship> ReportingRelationships { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
