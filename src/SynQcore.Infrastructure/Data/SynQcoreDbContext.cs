using System.Linq.Expressions;
using SynQcore.Domain.Common;
using Microsoft.EntityFrameworkCore;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Relationships;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SynQcore.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace SynQcore.Infrastructure.Data;

public class SynQcoreDbContext : IdentityDbContext<ApplicationUserEntity, IdentityRole<Guid>, Guid>
{
    public SynQcoreDbContext(DbContextOptions<SynQcoreDbContext> options) : base(options)
    {
    }
    
    // DbSets - Organization
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Position> Positions => Set<Position>();

    // DbSets - Communication
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<CommentLike> CommentLikes => Set<CommentLike>();
    public DbSet<Notification> Notifications => Set<Notification>();

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

