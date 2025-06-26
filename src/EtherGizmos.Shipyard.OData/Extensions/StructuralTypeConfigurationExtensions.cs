using EtherGizmos.Common.Utilities.Helpers;
using Microsoft.OData.ModelBuilder;
using System.Reflection;

namespace EtherGizmos.Shipyard.OData.Extensions;

/// <summary>
/// Provides extension methods for <see cref="StructuralTypeConfiguration{TEntityType}"/>.
/// </summary>
internal static class StructuralTypeConfigurationExtensions
{
    public static void IgnoreAll<TModel>(this StructuralTypeConfiguration<TModel> @this)
        where TModel : class
    {
        var method = typeof(StructuralTypeConfigurationExtensions)
            .GetMethod(
                nameof(IgnoreProperty),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        if (method is null)
            throw new InvalidOperationException($"Unable to locate {nameof(IgnoreProperty)}");

        foreach (var property in typeof(TModel).GetProperties())
        {
            //Due to a bug, we don't want to ignore dynamic properties. Adding them back kicks an error
            if (property.PropertyType.IsAssignableTo(typeof(IDictionary<string, object>)))
                continue;

            var genericMethod = method.MakeGenericMethod(typeof(TModel), property.PropertyType);
            genericMethod.Invoke(null, [@this, property]);
        }
    }

    private static void IgnoreProperty<TModel, TProperty>(StructuralTypeConfiguration<TModel> entity, PropertyInfo property)
        where TModel : class
    {
        var expression = ExpressionHelper.GetPropertyExpression<TModel, TProperty>(property);
        entity.Ignore(expression);
    }
}
