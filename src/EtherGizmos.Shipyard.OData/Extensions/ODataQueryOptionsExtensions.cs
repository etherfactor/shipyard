using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Models.Api.Errors;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.ModelBuilder.Annotations;
using Microsoft.OData.UriParser;
using System.Reflection;
using System.Text;

namespace EtherGizmos.Shipyard.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ODataQueryOptions{TEntity}"/>.
/// </summary>
public static class ODataQueryOptionsExtensions
{
    /// <summary>
    /// Ensures that the query options object can be applied to a single entity. Throws exceptions for any parameters that
    /// are not applicable. The exceptions take the form of <see cref="ReturnErrorException"/>, which should be caught
    /// by ReturnODataErrorFilter.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="this"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ReturnErrorException"></exception>
    public static void EnsureValidForSingle<TEntity>(
        this ODataQueryOptions<TEntity> @this)
    {
        ArgumentNullException.ThrowIfNull(@this);

        if (@this.Filter is not null)
        {
            new Error.UnsupportedOperation.QueryOptionNotApplicable()
                .AddDetail("$filter")
                .Return();
        }

        if (@this.OrderBy is not null)
        {
            new Error.UnsupportedOperation.QueryOptionNotApplicable()
                .AddDetail("$orderby")
                .Return();
        }

        if (@this.Top is not null)
        {
            new Error.UnsupportedOperation.QueryOptionNotApplicable()
                .AddDetail("$top")
                .Return();
        }

        if (@this.Skip is not null)
        {
            new Error.UnsupportedOperation.QueryOptionNotApplicable()
                .AddDetail("$skip")
                .Return();
        }

        if (@this.Count is not null)
        {
            new Error.UnsupportedOperation.QueryOptionNotApplicable()
                .AddDetail("$count")
                .Return();
        }
    }

    /// <summary>
    /// Gets a set of properties requested to be expanded.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>A set of properties to be expanded.</returns>
    public static IEnumerable<(string Path, PropertyInfo Property)> GetExpandedProperties<TEntity>(
        this ODataQueryOptions<TEntity> @this)
    {
        //Don't return anything if $select/$expand were unspecified
        if (@this.SelectExpand is null || @this.SelectExpand.SelectExpandClause is null)
        {
            yield break;
        }

        //Get the current type as a structured type
        //Needed to get properties
        var structuredType = (IEdmStructuredType)@this.Context.ElementType.AsElementType();

        //Iterate over expanded properties and return them
        var results = Sort(GetExpandPropertiesInternal(@this.Context.Model, structuredType, typeof(TEntity), @this.SelectExpand.SelectExpandClause));
        foreach (var result in results)
        {
            yield return result;
        }
    }

    private static IEnumerable<(string Path, PropertyInfo Property)> GetExpandPropertiesInternal(
        IEdmModel model,
        IEdmStructuredType edmType,
        Type clrType,
        SelectExpandClause selectExpand)
    {
        //Iterate over expanded properties
        foreach (var item in selectExpand.SelectedItems
            .Where(e => typeof(ExpandedNavigationSelectItem).IsAssignableFrom(e.GetType()))
            .Cast<ExpandedNavigationSelectItem>())
        {
            var thisLoopEdmType = edmType;
            var thisLoopClrType = clrType;

            //Find the property on the current type and get the CLR name
            //If that property is contained in a complex object, follow the navigation path first
            var propertyNames = new List<string>();
            PropertyInfo propertyInfo = null!;
            foreach (var navigationPath in item.PathToNavigationProperty)
            {
                var currentEdmProperty = thisLoopEdmType.FindProperty(navigationPath.Identifier);
                thisLoopEdmType = currentEdmProperty.Type.ToStructuredType();

                var currentPropertyName = model.GetClrPropertyName(currentEdmProperty);
                propertyNames.Add(currentPropertyName);

                var currentClrProperty = thisLoopClrType.GetProperty(currentPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
                thisLoopClrType = currentClrProperty.PropertyType;

                propertyInfo = currentClrProperty;
            }

            var propertyName = string.Join(".", propertyNames);

            //Return that property name
            yield return (propertyName, propertyInfo);

            //Then iterate over all expansions in that property
            foreach (var subProperty in GetExpandPropertiesInternal(model, (IEdmStructuredType)item.NavigationSource.Type.AsElementType(), propertyInfo.PropertyType, item.SelectAndExpand))
            {
                //Return the current property and all sub-expansions
                yield return ($"{propertyName}.{subProperty.Path}", subProperty.Property);
            }
        }
    }

    /// <summary>
    /// Gets a set of properties requested to be filtered.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>A set of properties to be filtered.</returns>
    public static IEnumerable<(string Path, PropertyInfo Property)> GetFilteredProperties<TEntity>(
        this ODataQueryOptions<TEntity> @this)
    {
        //Don't return anything if $filter was unspecified
        if (@this.Filter is null || @this.Filter.FilterClause is null)
        {
            yield break;
        }

        //Get the current type as a structured type
        //Needed to get properties
        var structuredType = (IEdmStructuredType)@this.Context.ElementType.AsElementType();

        //Iterate over filtered properties and return them
        var results = Sort(GetFilterPropertiesInternal(@this.Context.Model, structuredType, typeof(TEntity), @this.Filter.FilterClause.Expression));
        foreach (var result in results)
        {
            yield return result;
        }
    }

    private static IEnumerable<(string Path, PropertyInfo Property)> GetFilterPropertiesInternal(
        IEdmModel model,
        IEdmStructuredType edmType,
        Type clrType,
        QueryNode node)
    {
        foreach (var value in GetReferencedPropertiesInternal(model, edmType, clrType, node))
        {
            yield return value;
        }
    }

    /// <summary>
    /// Gets a set of properties requested to be ordered.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>A set of properties to be ordered.</returns>
    public static IEnumerable<(string Path, PropertyInfo Property)> GetOrderedProperties<TEntity>(
        this ODataQueryOptions<TEntity> @this)
    {
        //Don't return anything if $orderby was unspecified
        if (@this.OrderBy is null || @this.OrderBy.OrderByClause is null)
        {
            yield break;
        }

        //Get the current type as a structured type
        //Needed to get properties
        var structuredType = (IEdmStructuredType)@this.Context.ElementType.AsElementType();

        //Iterate over expanded properties and return them
        var results = Sort(GetOrderByPropertiesInternal(@this.Context.Model, structuredType, typeof(TEntity), @this.OrderBy.OrderByClause));
        foreach (var result in results)
        {
            yield return result;
        }
    }

    private static IEnumerable<(string Path, PropertyInfo Property)> GetOrderByPropertiesInternal(
        IEdmModel model,
        IEdmStructuredType edmType,
        Type clrType,
        OrderByClause order)
    {
        foreach (var value in GetReferencedPropertiesInternal(model, edmType, clrType, order.Expression))
        {
            yield return value;
        }

        if (order.ThenBy is not null)
        {
            foreach (var value in GetOrderByPropertiesInternal(model, edmType, clrType, order.ThenBy))
            {
                yield return value;
            }
        }
    }

    private static IEnumerable<(string Path, PropertyInfo Property)> GetReferencedPropertiesInternal(
        IEdmModel model,
        IEdmStructuredType edmType,
        Type clrType,
        QueryNode node)
    {
        switch (node)
        {
            //Handles property access
            case SingleValuePropertyAccessNode propertyNode:
                var singleEdmProperty = propertyNode.Property;
                var singleClrName = model.GetClrPropertyName(singleEdmProperty);

                var singleClrType = model.GetAnnotationValue<ClrTypeAnnotation>(edmType).ClrType;

                var singlePathChunks = singleClrName.Yield();

                var singleParent = propertyNode.Source as QueryNode;
                while (singleParent is SingleNavigationNode || singleParent is CollectionNavigationNode)
                {
                    if (singleParent is SingleNavigationNode singleParentSing)
                    {
                        var addPathChunk = model.GetClrPropertyName(singleParentSing.NavigationProperty);
                        singlePathChunks = singlePathChunks.Prepend(addPathChunk);

                        singleParent = singleParentSing.Source;
                    }
                    else if (singleParent is CollectionNavigationNode singleParentColl)
                    {
                        var addPathChunk = model.GetClrPropertyName(singleParentColl.NavigationProperty);
                        singlePathChunks = singlePathChunks.Prepend(addPathChunk);

                        singleParent = singleParentColl.Source;
                    }
                }

                var singlePathBuilder = new StringBuilder();

                var singleCurrentClrType = singleClrType;
                foreach (var propertyName in singlePathChunks)
                {
                    singlePathBuilder.Append($".{propertyName}");

                    var property = singleCurrentClrType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
                    singleCurrentClrType = property.PropertyType;

                    yield return (singlePathBuilder.ToString().TrimStart('.'), property);
                }
                break;

            //Handles basic casting and unwraps them
            case ConvertNode convertNode:
                foreach (var value in GetReferencedPropertiesInternal(model, edmType, clrType, convertNode.Source))
                {
                    yield return value;
                }
                break;

            //Handles both halves of boolean expressions
            case BinaryOperatorNode binaryNode:
                foreach (var value in GetReferencedPropertiesInternal(model, edmType, clrType, binaryNode.Left))
                {
                    yield return value;
                }
                foreach (var value in GetReferencedPropertiesInternal(model, edmType, clrType, binaryNode.Right))
                {
                    yield return value;
                }
                break;

            //Handles all function arguments
            case SingleValueFunctionCallNode functionNode:
                foreach (var argument in functionNode.Parameters.OfType<SingleValueNode>())
                {
                    foreach (var value in GetReferencedPropertiesInternal(model, edmType, clrType, argument))
                    {
                        yield return value;
                    }
                }
                break;

            //Handles properties referenced in any/all
            case LambdaNode lambdaNode when lambdaNode is AnyNode || lambdaNode is AllNode:
                if (lambdaNode.Source is not CollectionNavigationNode anyCollection)
                    break;

                var lambdaEdmProperty = anyCollection.NavigationProperty;
                var lambdaClrName = model.GetClrPropertyName(lambdaEdmProperty);
                var lambdaProperty = clrType.GetProperty(lambdaClrName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;

                var lambdaClrType = model.GetAnnotationValue<ClrTypeAnnotation>(edmType).ClrType;

                var lambdaPathChunks = Enumerable.Empty<string>();

                var lambdaParent = lambdaNode.Source as QueryNode;
                while (lambdaParent is SingleNavigationNode || lambdaParent is CollectionNavigationNode)
                {
                    if (lambdaParent is SingleNavigationNode lambdaParentSing)
                    {
                        var addPathChunk = model.GetClrPropertyName(lambdaParentSing.NavigationProperty);
                        lambdaPathChunks = lambdaPathChunks.Prepend(addPathChunk);

                        lambdaParent = lambdaParentSing.Source;
                    }
                    else if (lambdaParent is CollectionNavigationNode lambdaParentColl)
                    {
                        var addPathChunk = model.GetClrPropertyName(lambdaParentColl.NavigationProperty);
                        lambdaPathChunks = lambdaPathChunks.Prepend(addPathChunk);

                        lambdaParent = lambdaParentColl.Source;
                    }
                }

                var lambdaPathBuilder = new StringBuilder();

                var lambdaCurrentClrType = lambdaClrType;
                foreach (var propertyName in lambdaPathChunks)
                {
                    lambdaPathBuilder.Append($".{propertyName}");

                    var property = lambdaCurrentClrType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
                    lambdaCurrentClrType = property.PropertyType;

                    yield return (lambdaPathBuilder.ToString().TrimStart('.'), property);
                }

                var lambdaSubEdmType = lambdaEdmProperty
                    .Type.AsCollection().ElementType().ToStructuredType();
                var lambdaSubClrType = lambdaProperty.PropertyType.GetGenericArguments()[0];

                var prependPath = lambdaPathBuilder.ToString().TrimStart('.');

                foreach (var value in GetReferencedPropertiesInternal(model, lambdaSubEdmType, lambdaSubClrType, lambdaNode.Body))
                {
                    yield return ($"{prependPath}.{value.Path}", value.Property);
                }
                break;

            default:
                break;
        }
    }

    private static IEnumerable<(string Path, PropertyInfo Property)> Sort(
        IEnumerable<(string Path, PropertyInfo Property)> input)
    {
        return input.OrderBy(e => e.Path)
            .GroupBy(e => e.Path)
            .Select(e => (e.Key, e.First().Property));
    }
}
