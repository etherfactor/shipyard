using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Extensions;
using EtherGizmos.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Common.Services;

public class AuthorizationContext : DbContext
{
    public virtual DbSet<OAuth2Application> Applications { get; set; }

    public virtual DbSet<OAuth2Authorization> Authorizations { get; set; }

    public virtual DbSet<OAuth2Scope> Scopes { get; set; }

    public virtual DbSet<OAuth2Token> Tokens { get; set; }

    public AuthorizationContext(
        DbContextOptions<AuthorizationContext> options,
        IServiceProvider serviceProvider) : base(options)
    {
        serviceProvider.GetRequiredService<IOAuth2MigrationManager>()
            .EnsureMigratedAsync()
            .GetAwaiter()
            .GetResult();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //**********************************************************
        // Add Entities

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AuthorizationContext).Assembly);

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
