using EtherGizmos.Common.Configuration;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtherGizmos.Shipyard;

public static class IServiceCollectionArtifactExtensions
{
    public static IServiceCollection AddArtifactWriter(
        this IServiceCollection @this,
        Action<ArtifactOptions, IConfiguration> configureOptions)
    {
        @this.AddSingleton<IArtifactWriter, FileArtifactWriter>();

        @this.AddOptions<ArtifactOptions>()
            .Configure(configureOptions)
            .ValidateOnStart()
            .ValidateDataAnnotations();

        @this.AddDatabase()
            .AddDbContext<ArtifactContext>((services, opt) =>
            {
                opt.UseLazyLoadingProxies();
                opt.EnableSensitiveDataLogging();

                var dbOptions = services.GetRequiredService<IOptions<ArtifactOptions>>()
                    .Value;

                var connectionId = dbOptions.Database.ConnectionId;

                var resolver = services.GetRequiredService<IConnectionResolver>();
                var connection = resolver.GetDatabaseConnection(connectionId);

                connection.Match(
                    _ => throw new InvalidOperationException($"The connection {connectionId} is not a valid database connection."),
                    postgreSql =>
                    {
                        return opt.UseNpgsql(
                            postgreSql.ConnectionString,
                            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                    }
                );
            });

        return @this;
    }

    public static IServiceCollection AddArtifactReader(
        this IServiceCollection @this,
        Action<ArtifactOptions, IConfiguration> configureOptions)
    {
        @this.AddSingleton<IArtifactReader, FileArtifactReader>();

        @this.AddOptions<ArtifactOptions>()
            .Configure(configureOptions)
            .ValidateOnStart()
            .ValidateDataAnnotations();

        @this.AddDatabase()
            .AddDbContext<ArtifactContext>((services, opt) =>
            {
                opt.UseLazyLoadingProxies();
                opt.EnableSensitiveDataLogging();

                var dbOptions = services.GetRequiredService<IOptions<ArtifactOptions>>()
                    .Value;

                var connectionId = dbOptions.Database.ConnectionId;

                var resolver = services.GetRequiredService<IConnectionResolver>();
                var connection = resolver.GetDatabaseConnection(connectionId);

                connection.Match(
                    _ => throw new InvalidOperationException($"The connection {connectionId} is not a valid database connection."),
                    postgreSql =>
                    {
                        return opt.UseNpgsql(
                            postgreSql.ConnectionString,
                            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                    }
                );
            });

        return @this;
    }
}
