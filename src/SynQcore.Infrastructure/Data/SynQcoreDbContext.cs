using System.Linq.Expressions;
using SynQcore.Domain.Common;
using Microsoft.EntityFrameworkCore;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Relationships;
using SynQcore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SynQcore.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using SynQcore.Application.Common.Interfaces;

namespace SynQcore.Infrastructure.Data;

public class SynQcoreDbContext : IdentityDbContext<ApplicationUserEntity, IdentityRole<Guid>, Guid>, ISynQcoreDbContext
{
    public SynQcoreDbContext(DbContextOptions<SynQcoreDbContext> options) : base(options)
    {
    }
    
    // DbSets - Organization
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Position> Positions => Set<Position>();

    // Communication
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<CommentLike> CommentLikes => Set<CommentLike>();
    public DbSet<CommentMention> CommentMentions => Set<CommentMention>();
    public DbSet<Notification> Notifications => Set<Notification>();
    
    // Feed and Discovery (Fase 3.4)
    public DbSet<FeedEntry> FeedEntries => Set<FeedEntry>();
    public DbSet<UserInterest> UserInterests => Set<UserInterest>();
    
    // Knowledge Management
    public DbSet<KnowledgeCategory> KnowledgeCategories => Set<KnowledgeCategory>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PostTag> PostTags => Set<PostTag>();
    public DbSet<Endorsement> Endorsements => Set<Endorsement>();

    // Corporate Notifications (Fase 4.2)
    public DbSet<CorporateNotification> CorporateNotifications => Set<CorporateNotification>();
    public DbSet<NotificationDelivery> NotificationDeliveries => Set<NotificationDelivery>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();

    // DbSets - Relationships
    public DbSet<EmployeeDepartment> EmployeeDepartments => Set<EmployeeDepartment>();
    public DbSet<TeamMembership> TeamMemberships => Set<TeamMembership>();
    public DbSet<ReportingRelationship> ReportingRelationships => Set<ReportingRelationship>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Aplicar todas as configurações
        builder.ApplyConfigurationsFromAssembly(typeof(SynQcoreDbContext).Assembly);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetIsDeletedRestriction(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression GetIsDeletedRestriction(Type type)
    {
        var param = Expression.Parameter(type, "entity");
        var prop = Expression.Property(param, nameof(BaseEntity.IsDeleted));
        var condition = Expression.Equal(prop, Expression.Constant(false));
        return Expression.Lambda(condition, param);
    }
}

