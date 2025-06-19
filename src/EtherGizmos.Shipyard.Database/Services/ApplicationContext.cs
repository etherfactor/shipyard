using EtherGizmos.Shipyard.Database.Extensions;
using EtherGizmos.Shipyard.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Shipyard.Database.Services;

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

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset, DateTime>(
            app => app.UtcDateTime,
            db => new DateTimeOffset(db, TimeSpan.Zero)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset?, DateTime?>(
            app => app != null ? app.Value.UtcDateTime : null,
            db => db != null ? new DateTimeOffset((DateTime)db, TimeSpan.Zero) : null));
    }
}
