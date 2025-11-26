using EtherGizmos.Common.Providers;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace EtherGizmos.Common;

public static class IConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddRemappedEnvironmentVariables(
        this IConfigurationBuilder @this,
        params (Regex Remap, string Replacement)[] remaps)
    {
        @this.Add(new RemappedEnvironmentVariablesConfigurationSource()
        {
            Remaps = remaps,
        });

        return @this;
    }

    public static IConfigurationBuilder AddExpandedConnections(
        this IConfigurationBuilder @this,
        IConfigurationRoot configuration)
    {
        @this.Add(new ExpandedConnectionsVariablesConfigurationSource()
        {
            Configuration = configuration,
        });

        return @this;
    }
}
