using EtherGizmos.Common.Configuration.Providers;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace EtherGizmos.Common.Configuration;

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
}
