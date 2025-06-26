using EtherGizmos.Common.Utilities.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Database.Extensions;

/// <summary>
/// Provides extension methods for <see cref="EntityTypeBuilder{TEntity}"/>.
/// </summary>
public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Adds auditing columns to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    public static EntityTypeBuilder<TEntity> AuditProperties<TEntity>(this EntityTypeBuilder<TEntity> @this)
        where TEntity : class, IAuditable
    {
        @this.Property(e => e.CreatedAt)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql();

        @this.Property(e => e.CreatedByUserId)
            .HasColumnName("created_by_user_id");

        @this.Property(e => e.ModifiedAt)
            .HasColumnName("modified_at_utc");

        @this.Property(e => e.ModifiedByUserId)
            .HasColumnName("modified_by_user_id");

        return @this;
    }
}
