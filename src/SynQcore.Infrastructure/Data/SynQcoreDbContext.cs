using System.Linq.Expressions;
using SynQcore.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace SynQcore.Infrastructure.Data;

public class SynQcoreDbContext : DbContext
{
    public SynQcoreDbContext(DbContextOptions<SynQcoreDbContext> options) : base(options)
    {
    }

    // DbSets - Será populado com novas entidades
    // TODO: Adicionar novos DbSets aqui

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SynQcoreDbContext).Assembly);

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

