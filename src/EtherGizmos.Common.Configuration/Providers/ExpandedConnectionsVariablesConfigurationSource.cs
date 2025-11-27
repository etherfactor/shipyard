using Microsoft.Extensions.Configuration;

namespace EtherGizmos.Common.Providers;

internal class ExpandedConnectionsVariablesConfigurationSource : IConfigurationSource
{
    public IConfigurationRoot Configuration { get; set; } = null!;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ExpandedConnectionsVariablesConfigurationProvider(Configuration);
    }
}
