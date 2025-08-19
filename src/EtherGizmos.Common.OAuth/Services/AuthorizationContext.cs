using EtherGizmos.Common.Extensions;
using EtherGizmos.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Common.Services;

public class AuthorizationContext : DbContext
{
    public virtual DbSet<OAuth2Application> Applications { get; set; }

    public virtual DbSet<OAuth2Authorization> Authorizations { get; set; }

    public virtual DbSet<OAuth2Scope> Scopes { get; set; }

    public virtual DbSet<OAuth2Token> Tokens { get; set; }

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
            db => db != null ? new DateTimeOffset(db.Value, TimeSpan.Zero) : null));
    }
}
