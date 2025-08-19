using AutoMapper;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IMapper"/>.
/// </summary>
public static class IMapperExtensions
{
    private const AllowedQueryOptions SelectExpand = AllowedQueryOptions.Select | AllowedQueryOptions.Expand;
    private const AllowedQueryOptions NoSelectExpand = AllowedQueryOptions.All & ~SelectExpand;

    /// <summary>
    /// Maps an object from one form to another, explicitly (will only expand chosen properties, not all properties).
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="entity">The entity to map.</param>
    /// <returns>A wrapper to allow explicit mapping.</returns>
    public static IEntityMapBuilder<TFrom> MapExplicitly<TFrom>(
        this IMapper @this,
        TFrom entity)
        where TFrom : class
    {
        return new EntityMapBuilder<TFrom>
        {
            Mapper = @this,
            Value = entity,
        };
    }

    /// <summary>
    /// Allows explicit mapping.
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    public interface IEntityMapBuilder<TFrom>
        where TFrom : class
    {
        /// <summary>
        /// Maps explicitly to a type.
        /// </summary>
        /// <typeparam name="TTo">The resulting type.</typeparam>
        /// <param name="membersToExpand">The names of properties to expand.</param>
        /// <returns>The mapped type.</returns>
        IEntityMap<TFrom, TTo> To<TTo>()
            where TTo : class;
    }

    private record EntityMapBuilder<TFrom>
        : IEntityMapBuilder<TFrom>
        where TFrom : class
    {
        public required IMapper Mapper { get; init; }

        public required TFrom Value { get; init; }

        /// <inheritdoc/>
        public IEntityMap<TFrom, TTo> To<TTo>()
            where TTo : class
        {
            return new EntityMap<TFrom, TTo>
            {
                Mapper = Mapper,
                Value = Value,
            };
        }
    }

    public interface IEntityMap<TFrom, TTo>
        where TFrom : class
        where TTo : class
    {
        IEntityMap<TFrom, TTo> ApplyQueryOptions(ODataQueryOptions<TTo> queryOptions);

        object Execute();

        IEntityMap<TFrom, TTo> Expand(params string[] membersToExpand);
    }

    private record EntityMap<TFrom, TTo>
        : IEntityMap<TFrom, TTo>
        where TFrom : class
        where TTo : class
    {
        public required IMapper Mapper { get; init; }

        public required TFrom Value { get; init; }

        public IEnumerable<string> MembersToExpand { get; init; } = [];

        public IEnumerable<ODataQueryOptions<TTo>> QueryOptions { get; init; } = [];

        public IEntityMap<TFrom, TTo> ApplyQueryOptions(
            ODataQueryOptions<TTo> queryOptions)
        {
            return this with
            {
                QueryOptions = QueryOptions.Append(queryOptions),
            };
        }

        public IEntityMap<TFrom, TTo> Expand(
            params string[] membersToExpand)
        {
            return this with
            {
                MembersToExpand = MembersToExpand.Concat(membersToExpand),
            };
        }

        public object Execute()
        {
            var membersToExpand = MembersToExpand
                .Concat(QueryOptions.SelectMany(qopt => qopt.GetExpandedProperties().Select(e => e.Path)))
                .Concat(QueryOptions.SelectMany(qopt => qopt.GetFilteredProperties().Select(e => e.Path)))
                .Concat(QueryOptions.SelectMany(qopt => qopt.GetOrderedProperties().Select(e => e.Path)))
                .Distinct();

            var queryable = Value.Yield().AsQueryable();
            var projected = Mapper.ProjectTo<TTo>(queryable, null, [.. membersToExpand]);
            object record = projected.ToList().Single();

            foreach (var queryOptions in QueryOptions)
            {
                record = queryOptions.ApplyTo(record, new ODataQuerySettings());
            }

            return record;
        }
    }

    public static IProjectionMapBuilder<TFrom> MapExplicitly<TFrom>(
        this IMapper @this,
        IQueryable<TFrom> queryable)
        where TFrom : class
    {
        return new ProjectionMapBuilder<TFrom>
        {
            Mapper = @this,
            Value = queryable,
        };
    }

    public interface IProjectionMapBuilder<TFrom>
        where TFrom : class
    {
        IProjectionMap<TFrom, TTo> To<TTo>()
            where TTo : class;
    }

    private record ProjectionMapBuilder<TFrom>
        : IProjectionMapBuilder<TFrom>
        where TFrom : class
    {
        public required IMapper Mapper { get; init; }

        public required IQueryable<TFrom> Value { get; init; }

        public IProjectionMap<TFrom, TTo> To<TTo>()
            where TTo : class
        {
            return new ProjectionMap<TFrom, TTo>
            {
                Mapper = Mapper,
                Value = Value,
            };
        }
    }

    public interface IProjectionMap<TFrom, TTo>
        where TFrom : class
        where TTo : class
    {
        IProjectionMap<TFrom, TTo> ApplyQueryOptions(ODataQueryOptions<TTo> queryOptions);

        Task<IQueryable> ExecuteAsync(CancellationToken cancellationToken = default);

        IProjectionMap<TFrom, TTo> Expand(params string[] membersToExpand);
    }

    private record ProjectionMap<TFrom, TTo>
        : IProjectionMap<TFrom, TTo>
        where TFrom : class
        where TTo : class
    {
        public required IMapper Mapper { get; init; }

        public required IQueryable<TFrom> Value { get; init; }

        public IEnumerable<string> MembersToExpand { get; init; } = [];

        public IEnumerable<ODataQueryOptions<TTo>> QueryOptions { get; init; } = [];

        public IProjectionMap<TFrom, TTo> ApplyQueryOptions(
            ODataQueryOptions<TTo> queryOptions)
        {
            return this with
            {
                QueryOptions = QueryOptions.Append(queryOptions),
            };
        }

        public IProjectionMap<TFrom, TTo> Expand(
            params string[] membersToExpand)
        {
            return this with
            {
                MembersToExpand = MembersToExpand.Concat(membersToExpand),
            };
        }

        public async Task<IQueryable> ExecuteAsync(
            CancellationToken cancellationToken = default)
        {
            var membersToExpand = MembersToExpand
                .Concat(QueryOptions.SelectMany(qopt => qopt.GetExpandedProperties().Select(e => e.Path)))
                .Concat(QueryOptions.SelectMany(qopt => qopt.GetFilteredProperties().Select(e => e.Path)))
                .Concat(QueryOptions.SelectMany(qopt => qopt.GetOrderedProperties().Select(e => e.Path)))
                .Distinct();

            var queryable1 = Mapper.ProjectTo<TTo>(Value, null, [.. membersToExpand]);

            foreach (var queryOptions in QueryOptions)
            {
                queryable1 = (IQueryable<TTo>)queryOptions.ApplyTo(queryable1, ignoreQueryOptions: SelectExpand);
            }

            IQueryable queryable2;
            if (queryable1.GetType().GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)))
            {
                queryable2 = (await queryable1.ToListAsync(cancellationToken: cancellationToken)).AsQueryable();
            }
            else
            {
                queryable2 = queryable1.ToList().AsQueryable();
            }

            foreach (var queryOptions in QueryOptions)
            {
                queryable2 = queryOptions.ApplyTo(queryable2, ignoreQueryOptions: NoSelectExpand);
            }

            return queryable2;
        }
    }

    /// <summary>
    /// Merges records of one type into a record of another type, mapping the former objects into the latter.
    /// </summary>
    /// <typeparam name="TTo">The resulting type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="entity">The entity to map.</param>
    /// <returns>A wrapper to allow merging.</returns>
    public static IEntityMergeBuilder<TTo> MergeInto<TTo>(
        this IMapper @this,
        TTo entity)
        where TTo : class
    {
        return new EntityMerge<TTo>
        {
            Mapper = @this,
            Value = entity,
        };
    }

    /// <summary>
    /// Simplifies record merging.
    /// </summary>
    /// <typeparam name="TTo">The resulting type.</typeparam>
    public interface IEntityMergeBuilder<TTo>
        where TTo : class
    {
        /// <summary>
        /// Merges records into the original.
        /// </summary>
        /// <typeparam name="TFrom">The initial type.</typeparam>
        /// <param name="records">The objects to merge.</param>
        /// <returns>The mapped type.</returns>
        IEntityMerge<TTo> Using(params object[] records);
    }

    public interface IEntityMerge<TTo>
        where TTo : class
    {
        TTo Execute();
    }

    private record EntityMerge<TTo>
        : IEntityMergeBuilder<TTo>,
        IEntityMerge<TTo>
        where TTo : class
    {
        public required IMapper Mapper { get; init; }

        public required TTo Value { get; init; }

        public object[] From { get; init; } = [];

        /// <inheritdoc/>
        public IEntityMerge<TTo> Using(
            params object[] records)
        {
            return this with
            {
                From = records
            };
        }

        /// <inheritdoc/>
        public TTo Execute()
        {
            return From.Aggregate(Value, (aggregate, record) =>
                Mapper.Map(record, aggregate));
        }
    }
}
