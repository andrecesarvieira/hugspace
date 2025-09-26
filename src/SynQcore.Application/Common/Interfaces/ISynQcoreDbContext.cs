using Microsoft.EntityFrameworkCore;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Domain.Entities.Communication;
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

    // Knowledge Management entities
    DbSet<KnowledgeCategory> KnowledgeCategories { get; }
    DbSet<Tag> Tags { get; }
    DbSet<PostTag> PostTags { get; }
    DbSet<Endorsement> Endorsements { get; }

    // Relationship entities
    DbSet<EmployeeDepartment> EmployeeDepartments { get; }
    DbSet<TeamMembership> TeamMemberships { get; }
    DbSet<ReportingRelationship> ReportingRelationships { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}