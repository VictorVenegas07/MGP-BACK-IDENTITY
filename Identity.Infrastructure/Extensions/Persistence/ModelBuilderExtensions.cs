using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Identity.Infrastructure.Extensions.Persistence
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder AppendGlobalQueryFilter<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> filter)
        {
            var entities = modelBuilder.Model.GetEntityTypes()
                .Where(e => e.BaseType is null && e.ClrType.GetInterface(typeof(TInterface).Name) is not null)
                .Select(e => e.ClrType);

            foreach (var entity in entities)
            {
                var parameterType = Expression.Parameter(modelBuilder.Entity(entity).Metadata.ClrType);
                var filterBody = ReplacingExpressionVisitor.Replace(filter.Parameters.Single(), parameterType, filter.Body);

                if (modelBuilder.Entity(entity).Metadata.GetQueryFilter() is { } existingFilter)
                {
                    var existingFilterBody = ReplacingExpressionVisitor.Replace(existingFilter.Parameters.Single(), parameterType, existingFilter.Body);
                    filterBody = Expression.AndAlso(existingFilterBody, filterBody);
                }

                modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(filterBody, parameterType));
            }

            return modelBuilder;
        }

        public static ModelBuilder ApplyEntityConfigurations(this ModelBuilder modelBuilder)
        {
            
           

            return modelBuilder;
        }
    }
}
