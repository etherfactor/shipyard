using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Converters;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Services;

public class ApplicationContext : DbContext
{
    private readonly IUserContext _userContext;

    public IUserContext UserContext => _userContext;

    public virtual DbSet<AclCarrier> AclCarriers { get; set; }

    public virtual DbSet<AclEntry> AclEntries { get; set; }

    public virtual DbSet<AclPackage> AclPackages { get; set; }

    public virtual DbSet<AclRole> AclRoles { get; set; }

    public virtual DbSet<AclUser> AclUsers { get; set; }

    public virtual DbSet<Carrier> Carriers { get; set; }

    public virtual DbSet<CarrierExecution> CarrierExecutions { get; set; }

    public virtual DbSet<CarrierExecutionArtifact> CarrierExecutionArtifacts { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleUser> RoleUsers { get; set; }

    public virtual DbSet<StatusType> StatusTypes { get; set; }

    public virtual DbSet<TrackingUpdate> TrackingUpdates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public ApplicationContext(
        DbContextOptions<ApplicationContext> options,
        IMigrationManager migrationManager,
        IUserContext userContext) : base(options)
    {
        _userContext = userContext;

        migrationManager.EnsureMigratedAsync()
            .GetAwaiter()
            .GetResult();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ReplaceService<IModelCacheKeyFactory, UserContextModelCacheKeyFactory>();
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

        modelBuilder.AddGlobalValueConverter(new ValueConverter<ArtifactUri, string>(
            app => app.Value,
            db => db != null ? new ArtifactUri(db) : default));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<IDictionary<string, object?>, string>(
            app => JsonSerializer.Serialize(app, jsonOptions),
            db => JsonSerializer.Deserialize<IDictionary<string, object?>>(db, jsonOptions)!),
            new ValueComparer<IDictionary<string, object?>>(
                (a, b) => JsonSerializer.Serialize(a, jsonOptions) == JsonSerializer.Serialize(b, jsonOptions),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => new Dictionary<string, object?>(c)));

        //**********************************************************
        // Add Query Filters

        var userId = _userContext.UserId;

        if (userId is not null)
        {
            modelBuilder.Entity<Package>()
                .HasQueryFilter(e => AclPackages.Any(e =>
                    e.PrincipalUserId == userId
                    && e.PermissionId == PermissionId.Read
                    && e.IsGrant == 1));
        }
        else
        {
            modelBuilder.Entity<Package>()
                .HasQueryFilter(e => false);
        }
    }
}
