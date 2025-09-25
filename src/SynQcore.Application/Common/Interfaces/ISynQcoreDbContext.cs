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
    DbSet<PostLike> PostLikes { get; }
    DbSet<CommentLike> CommentLikes { get; }
    DbSet<Notification> Notifications { get; }

    // Relationship entities
    DbSet<EmployeeDepartment> EmployeeDepartments { get; }
    DbSet<TeamMembership> TeamMemberships { get; }
    DbSet<ReportingRelationship> ReportingRelationships { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}