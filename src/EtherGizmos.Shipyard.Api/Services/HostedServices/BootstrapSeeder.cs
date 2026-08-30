using EtherGizmos.Shipyard.Abstractions;

namespace EtherGizmos.Shipyard.Services.HostedServices;

public class BootstrapSeeder : IHostedService
{
    private IEnumerable<IBootstrapper> _bootstrappers;

    public BootstrapSeeder(
        IEnumerable<IBootstrapper> bootstrappers)
    {
        _bootstrappers = bootstrappers;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sorted = _bootstrappers.OrderBy(e => e.Order);
        foreach (var bootstrapper in sorted)
        {
            await bootstrapper.ExecuteAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
