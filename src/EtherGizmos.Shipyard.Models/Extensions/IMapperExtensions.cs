using AutoMapper;

namespace EtherGizmos.Shipyard.Models.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IMapper"/>.
/// </summary>
public static class IMapperExtensions
{
    /// <summary>
    /// Allows explicit mapping.
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    public interface IMapExplicitly<TFrom>
    {
        /// <summary>
        /// Maps explicitly to a type.
        /// </summary>
        /// <typeparam name="TTo">The resulting type.</typeparam>
        /// <param name="membersToExpand">The names of properties to expand.</param>
        /// <returns>The mapped type.</returns>
        TTo To<TTo>(params string[] membersToExpand);
    }

    private class ValueMapExplicitly<TFrom> : IMapExplicitly<TFrom>
    {
        private IMapper Mapper { get; }

        private TFrom Value { get; }

        internal ValueMapExplicitly(IMapper mapper, TFrom value)
        {
            Mapper = mapper;
            Value = value;
        }

        /// <inheritdoc/>
        public TTo To<TTo>(params string[] membersToExpand)
        {
            IQueryable<TFrom> queryable = Value.Yield().AsQueryable();
            IQueryable<TTo> projected = Mapper.ProjectTo<TTo>(queryable, null, membersToExpand);
            var list = projected.ToList();

            return list.Single();
        }
    }

    /// <summary>
    /// Maps an object from one form to another, explicitly (will only expand chosen properties, not all properties).
    /// </summary>
    /// <typeparam name="TTo">The initial type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="entity">The entity to map.</param>
    /// <returns>A wrapper to allow explicit mapping.</returns>
    public static IMapExplicitly<TTo> MapExplicitly<TTo>(this IMapper @this, TTo entity)
    {
        return new ValueMapExplicitly<TTo>(@this, entity);
    }

    /// <summary>
    /// Simplifies record merging.
    /// </summary>
    /// <typeparam name="TTo">The resulting type.</typeparam>
    public interface IMergeInto<TTo>
    {
        /// <summary>
        /// Merges records into the original.
        /// </summary>
        /// <typeparam name="TFrom">The initial type.</typeparam>
        /// <param name="records">The objects to merge.</param>
        /// <returns>The mapped type.</returns>
        TTo Using<TFrom>(params TFrom[] records);
    }

    private class ValueMergeInto<TTo> : IMergeInto<TTo>
    {
        private IMapper Mapper { get; }

        private TTo Value { get; }

        internal ValueMergeInto(IMapper mapper, TTo value)
        {
            Mapper = mapper;
            Value = value;
        }

        /// <inheritdoc/>
        public TTo Using<TFrom>(params TFrom[] records)
        {
            return records.Aggregate(Value, (aggregate, record) => Mapper.Map(record, aggregate));
        }
    }

    /// <summary>
    /// Merges records of one type into a record of another type, mapping the former objects into the latter.
    /// </summary>
    /// <typeparam name="TTo">The resulting type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="entity">The entity to map.</param>
    /// <returns>A wrapper to allow merging.</returns>
    public static IMergeInto<TTo> MergeInto<TTo>(this IMapper @this, TTo entity)
    {
        return new ValueMergeInto<TTo>(@this, entity);
    }
}
