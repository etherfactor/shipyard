using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EtherGizmos.Shipyard;

public class ConfiguredWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private readonly IDictionary<string, string?> _configuration;

    public ConfiguredWebApplicationFactory(
        IDictionary<string, string?> configuration)
    {
        _configuration = configuration;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(_configuration);
        });

        return base.CreateHost(builder);
    }
}
