using EtherGizmos.Common;
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

        @this.AddDbContext<ArtifactContext>((services, opt) =>
            {
                opt.UseLazyLoadingProxies();
                opt.EnableSensitiveDataLogging();

                var dbOptions = services.GetRequiredService<IOptions<ArtifactOptions>>()
                    .Value;

                var connectionId = dbOptions.Database.ConnectionId;

                opt.UseConnection(services, connectionId);
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

        @this.AddDbContext<ArtifactContext>((services, opt) =>
            {
                opt.UseLazyLoadingProxies();
                opt.EnableSensitiveDataLogging();

                var dbOptions = services.GetRequiredService<IOptions<ArtifactOptions>>()
                    .Value;

                var connectionId = dbOptions.Database.ConnectionId;

                opt.UseConnection(services, connectionId);
            });

        return @this;
    }
}
