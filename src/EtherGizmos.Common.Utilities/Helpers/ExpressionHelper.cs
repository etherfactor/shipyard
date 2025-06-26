using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.Common.Utilities.Helpers;

public static class ExpressionHelper
{
    public static Expression<Func<T, TProperty>> GetPropertyExpression<T, TProperty>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda<Func<T, TProperty>>(property, parameter);
        return lambda;
    }

    public static Expression<Func<T, TProperty>> GetPropertyExpression<T, TProperty>(PropertyInfo propertyInfo)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda<Func<T, TProperty>>(property, parameter);
        return lambda;
    }
}
