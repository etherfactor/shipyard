using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Shipyard.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
    extension(ModelBuilder @this)
    {
        /// <summary>
        /// Adds a value converter to all properties of a given type.
        /// </summary>
        /// <typeparam name="TProvider">The database type.</typeparam>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <param name="converter">The converter to use.</param>
        /// <param name="comparer">Determines if two values are equivalent.</param>
        /// <returns>Itself.</returns>
        public ModelBuilder AddGlobalValueConverter<TProvider, TModel>(
            ValueConverter<TModel, TProvider> converter,
            ValueComparer<TModel>? comparer = null)
        {
            foreach (var entityType in @this.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(TModel))
                    {
                        //Don't overwrite existing value converters
                        if (property.GetValueConverter() is not null)
                            continue;

                        property.SetValueConverter(converter);

                        if (comparer is not null)
                        {
                            property.SetValueComparer(comparer);
                        }
                    }
                }
            }

            return @this;
        }

        /// <summary>
        /// Adds a column type declaration to all columns of a given property type.
        /// </summary>
        /// <typeparam name="TProperty">The property type type.</typeparam>
        /// <param name="columnType">The column type.</param>
        /// <returns>Itself.</returns>
        public ModelBuilder AddGlobalColumnType<TProperty>(
            string columnType)
        {
            foreach (var entityType in @this.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(TProperty))
                    {
                        property.SetColumnType(columnType);
                    }
                }
            }

            return @this;
        }
    }
}
