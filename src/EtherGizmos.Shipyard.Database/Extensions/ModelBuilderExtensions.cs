using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Shipyard.Database.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds a value converter to all properties of a given type.
    /// </summary>
    /// <typeparam name="TProvider">The database type.</typeparam>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="converter">The converter to use.</param>
    /// <returns>Itself.</returns>
    public static ModelBuilder AddGlobalValueConverter<TProvider, TModel>(this ModelBuilder @this, ValueConverter<TModel, TProvider> converter)
    {
        foreach (var entityType in @this.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(TModel))
                {
                    property.SetValueConverter(converter);
                }
            }
        }

        return @this;
    }
}
