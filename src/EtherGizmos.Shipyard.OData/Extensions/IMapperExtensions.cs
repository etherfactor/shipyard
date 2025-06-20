using Microsoft.AspNetCore.OData.Query;

namespace EtherGizmos.Shipyard.OData.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IMapper"/>.
/// </summary>
public static class IMapperExtensions
{
    /// <summary>
    /// Explicitly maps an object and applies OData query options.
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    /// <typeparam name="TTo">The final type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="object">The object to map.</param>
    /// <param name="queryOptions">The OData query options.</param>
    /// <returns>The mapped object. (Note: cannot be cast to <typeparamref name="TTo"/> if $select/$expand are used.)</returns>
    public static object MapExplicitlyAndApplyQueryOptions<TFrom, TTo>(this IMapper @this, TFrom @object, ODataQueryOptions<TTo> queryOptions)
        where TFrom : class
        where TTo : class
    {
        var membersToExpand = queryOptions.GetExpandedProperties().ToArray();

        var mapped = @this.MapExplicitly(@object).To<TTo>(membersToExpand);
        object finished = queryOptions.ApplyTo(mapped, new ODataQuerySettings());

        return finished;
    }

    /// <summary>
    /// Explicitly maps a queryable and applies OData query options.
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    /// <typeparam name="TTo">The final type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="queryable">The queryable to map.</param>
    /// <param name="queryOptions">The OData query options.</param>
    /// <returns>The mapped queryable. (Note: cannot be cast to <see cref="IQueryable{TFrom}"/> if $select/$expand are used.)</returns>
    public static async Task<IQueryable> MapExplicitlyAndApplyQueryOptions<TFrom, TTo>(this IMapper @this, IQueryable<TFrom> queryable, ODataQueryOptions<TTo> queryOptions)
        where TFrom : class
        where TTo : class
    {
        var expanded = queryOptions.GetExpandedProperties().ToArray();
        var result = @this.ProjectTo<TTo>(queryable, null, expanded);

        return await ApplyFixedQueryOptions(result, queryOptions);
    }

    /// <summary>
    /// Applies OData query options, filtering in the database and performing $select/$expand in-memory. Mitigates various
    /// LINQ-to-SQL errors.
    /// </summary>
    /// <typeparam name="TEntity">The initial type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="queryOptions">The OData query options.</param>
    /// <returns>The result queryable. (Note: cannot be cast to <see cref="IQueryable{TFrom}"/> if $select/$expand are used.)</returns>
    public static async Task<IQueryable> ApplyFixedQueryOptions<TEntity>(IQueryable<TEntity> queryable, ODataQueryOptions<TEntity> queryOptions)
        where TEntity : class
    {
        var noSelectExpand = AllowedQueryOptions.Select | AllowedQueryOptions.Expand;
        var onlySelectExpand = AllowedQueryOptions.All & ~noSelectExpand;

        var noSelectExpandQueryable = (IQueryable<TEntity>)queryOptions.ApplyTo(queryable, noSelectExpand);

        var noSelectExpandList = await noSelectExpandQueryable.ToListAsync();
        var finished = queryOptions.ApplyTo(noSelectExpandList.AsQueryable(), onlySelectExpand);

        return finished;
    }
}
