using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Services;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EtherGizmos.Shipyard;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection @this)
    {
        @this.AddOptions<DatabaseReferenceOptions>()
            .ValidateOnStart()
            .ValidateDataAnnotations();

        @this.AddDbContext<ApplicationContext>((services, opt) =>
            {
                opt.UseLazyLoadingProxies();
                opt.EnableSensitiveDataLogging();

                var dbOptions = services.GetRequiredService<IOptions<DatabaseReferenceOptions>>()
                    .Value;

                var connectionId = dbOptions.ConnectionId;

                var resolver = services.GetRequiredService<IConnectionResolver>();

                opt.UseConnection(services, connectionId);
            });

        @this.AddMigrations(typeof(ApplicationContext).Assembly);

        @this.AddHttpContextAccessor();
        @this.TryAddSingleton<IUserContext, UserContext>();

        return @this;
    }
}
