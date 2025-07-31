using System.Linq.Expressions;
using System.Reflection;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.Data.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies soft delete query filters to all entities that implement ISoftDelete
    /// </summary>
    public static void ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                entityType.AddSoftDeleteQueryFilter();
            }
        }
    }
    public static void AddQueryFilter<T>(this Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType, Expression<Func<T, bool>> expression)
    {
        var parameterType = Expression.Parameter(entityType.ClrType);
        var expressionFilter = ReplacingExpressionVisitor.Replace(
            expression.Parameters.Single(), parameterType, expression.Body);

        var currentQueryFilter = entityType.GetQueryFilter();
        if (currentQueryFilter != null)
        {
            var currentExpressionFilter = ReplacingExpressionVisitor.Replace(
                currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);
            expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
        }

        var lambdaExpression = Expression.Lambda(expressionFilter, parameterType);
        entityType.SetQueryFilter(lambdaExpression);
    }
    private static void AddSoftDeleteQueryFilter(this IMutableEntityType entityType)
    {
        var methodToCall = typeof(ModelBuilderExtensions)
            .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(entityType.ClrType);

        var filter = methodToCall.Invoke(null, null);
        entityType.SetQueryFilter((LambdaExpression)filter!);
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : class, ISoftDelete
    {
        Expression<Func<TEntity, bool>> filter = e => !e.IsDeleted;
        return filter;
    }
}