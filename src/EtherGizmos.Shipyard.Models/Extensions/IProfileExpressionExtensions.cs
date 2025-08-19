using AutoMapper;
using AutoMapper.Configuration;
using System.Linq.Expressions;

namespace EtherGizmos.Shipyard.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IProfileExpression"/>.
/// </summary>
internal static class IProfileExpressionExtensions
{
    /// <summary>
    /// Ignores all members, until they are overridden.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    internal static IMappingExpression<TSource, TDestination> IgnoreAllMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> @this)
    {
        var destinationType = typeof(TDestination);

        foreach (var property in destinationType.GetProperties())
            @this.ForMember(property.Name, opt => opt.Ignore());

        return @this;
    }

    /// <summary>
    /// Maps a member, pulling out the source into a dedicated argument.
    /// </summary>
    /// <typeparam name="TSource">The type of source entity.</typeparam>
    /// <typeparam name="TSourceMember">The type of source entity member.</typeparam>
    /// <typeparam name="TDestination">The type of destination entity.</typeparam>
    /// <typeparam name="TDestinationMember">The type of destination entity member.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="destinationMember">The destination selector.</param>
    /// <param name="sourceMember">The source selector.</param>
    /// <param name="memberOptions">Additional configuration.</param>
    /// <returns>Itself.</returns>
    internal static IMappingExpression<TSource, TDestination> MapMember<TSource, TSourceMember, TDestination, TDestinationMember>(
        this IMappingExpression<TSource, TDestination> @this, Expression<Func<TDestination, TDestinationMember>> destinationMember,
        Expression<Func<TSource, TSourceMember>> sourceMember, Action<IMemberConfigurationExpression<TSource, TDestination, TDestinationMember>>? memberOptions = null)
    {
        //Map a member, applying the custom options, followed by the source path
        @this.ForMember(destinationMember, configure =>
        {
            if (memberOptions != null)
                memberOptions(configure);

            configure.MapFrom(sourceMember);
        });

        return @this;
    }

    /// <summary>
    /// Maps a path, pulling out the source into a dedicated argument.
    /// </summary>
    /// <typeparam name="TSource">The type of source entity.</typeparam>
    /// <typeparam name="TSourceMember">The type of source entity member.</typeparam>
    /// <typeparam name="TDestination">The type of destination entity.</typeparam>
    /// <typeparam name="TDestinationMember">The type of destination entity member.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="destinationPath">The destination selector.</param>
    /// <param name="sourcePath">The source selector.</param>
    /// <param name="pathOptions">Additional configuration.</param>
    /// <returns>Itself.</returns>
    internal static IMappingExpression<TSource, TDestination> MapPath<TSource, TSourceMember, TDestination, TDestinationMember>(
        this IMappingExpression<TSource, TDestination> @this, Expression<Func<TDestination, TDestinationMember>> destinationPath,
        Expression<Func<TSource, TSourceMember>> sourcePath, Action<IPathConfigurationExpression<TSource, TDestination, TDestinationMember>>? pathOptions = null)
    {
        //Map a path, applying the custom options, followed by the source path
        @this.ForPath(destinationPath, configure =>
        {
            if (pathOptions != null)
                pathOptions(configure);

            configure.MapFrom(sourcePath);
        });

        return @this;
    }
}
