using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public abstract class ScrapingStep
{
    public ScrapingStepType Type { get; set; }

    public abstract Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default);

    protected internal abstract Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default);
}
