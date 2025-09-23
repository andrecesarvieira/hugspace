using System.Linq.Expressions;
using EnterpriseHub.Domain.Common;
using EnterpriseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Infrastructure.Data;

public class EnterpriseHubDbContext : DbContext
{
    public EnterpriseHubDbContext(DbContextOptions<EnterpriseHubDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<PostLike> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EnterpriseHubDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
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

