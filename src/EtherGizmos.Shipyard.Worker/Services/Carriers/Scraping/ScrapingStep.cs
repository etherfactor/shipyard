using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public abstract class ScrapingStep
{
    internal ILogger Logger { get; set; } = NullLogger.Instance;

    public abstract Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default);

    protected internal abstract Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default);
}
