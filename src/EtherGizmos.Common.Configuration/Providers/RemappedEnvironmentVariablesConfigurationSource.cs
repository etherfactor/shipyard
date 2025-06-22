using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace EtherGizmos.Configuration.Providers;

internal sealed class RemappedEnvironmentVariablesConfigurationSource : IConfigurationSource
{
    public IEnumerable<(Regex Match, string Replacement)> Remaps { get; set; } = [];

    public IConfigurationProvider Build(
        IConfigurationBuilder builder)
    {
        return new RemappedEnvironmentVariablesConfigurationProvider(Remaps);
    }
}
