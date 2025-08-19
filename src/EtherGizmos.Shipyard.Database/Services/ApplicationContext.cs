using EtherGizmos.Common.Converters;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Services;

public class ApplicationContext : DbContext
{
    public virtual DbSet<Carrier> Carriers { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<StatusType> StatusTypes { get; set; }

    public virtual DbSet<TrackingUpdate> TrackingUpdates { get; set; }

    public ApplicationContext(
        DbContextOptions<ApplicationContext> options,
        IMigrationManager migrationManager) : base(options)
    {
        migrationManager.EnsureMigrated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //**********************************************************
        // Add Entities

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationContext).Assembly);

        //**********************************************************
        // Add Value Converters

        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new ObjectToInferredTypesConverter());

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset, DateTime>(
            app => app.UtcDateTime,
            db => new DateTimeOffset(db, TimeSpan.Zero)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset?, DateTime?>(
            app => app != null ? app.Value.UtcDateTime : null,
            db => db != null ? new DateTimeOffset((DateTime)db, TimeSpan.Zero) : null));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<IDictionary<string, object?>, string>(
            app => JsonSerializer.Serialize(app, jsonOptions),
            db => JsonSerializer.Deserialize<IDictionary<string, object?>>(db, jsonOptions)!),
            new ValueComparer<IDictionary<string, object?>>(
                (a, b) => JsonSerializer.Serialize(a, jsonOptions) == JsonSerializer.Serialize(b, jsonOptions),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => new Dictionary<string, object?>(c)));
    }
}
