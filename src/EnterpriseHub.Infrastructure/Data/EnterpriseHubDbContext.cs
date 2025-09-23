using System.Linq.Expressions;
using EnterpriseHub.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Infrastructure.Data;

public class EnterpriseHubDbContext : DbContext
{
    public EnterpriseHubDbContext(DbContextOptions<EnterpriseHubDbContext> options) : base(options)
    {
    }

    // DbSets - Será populado com novas entidades
    // TODO: Adicionar novos DbSets aqui

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

