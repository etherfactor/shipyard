using EtherGizmos.Shipyard.Worker.Services.WebDrivers;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public abstract class ScrapingStep
{
    public ScrapingStepType Type { get; set; }

    public abstract Task Apply(IBrowserClient client, CancellationToken cancellationToken = default);
}
