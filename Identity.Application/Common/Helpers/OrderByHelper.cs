using Identity.Domain.Common.Helpers;
using Identity.Domain.Entities.Base;
using System.Linq.Expressions;

namespace Identity.Application.Common.Helpers;

public static class OrderByHelper<T> where T : BaseEntity<int>
{
public static Func<IQueryable<T>, IOrderedQueryable<T>> BuildOrderBy(IEnumerable<OrderByField>? orderByFields)
{
    var orderByBuilder = new OrderByBuilder<T>();

    if (orderByFields == null || !orderByFields.Any())
    {
        return query => query.OrderBy(pmt => pmt.CreatedAt);
    }

    foreach (var orderByField in orderByFields)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression propertyAccess = parameter;

        foreach (var propName in orderByField.Campo.Split('.'))
        {
            propertyAccess = Expression.PropertyOrField(propertyAccess, propName);
        }

        var keySelector = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);

        if (orderByField.EsAscendente)
        {
            orderByBuilder.OrderBy(keySelector);
        }
        else
        {
            orderByBuilder.OrderByDescending(keySelector);
        }
    }

    return orderByBuilder.Build();
}

}
