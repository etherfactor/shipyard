using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public abstract class ScrapingStep
{
    private readonly IList<TrackingResultDetail> _steps = [];
    private DateTimeOffset? _eta;

    internal ILogger Logger { get; set; } = NullLogger.Instance;

    internal DateTimeOffset? Eta => _eta;

    internal IReadOnlyList<TrackingResultDetail> Updates => _steps.AsReadOnly();

    protected void SetEta(DateTimeOffset eta)
    {
        _eta = eta;
    }

    protected void AddEvent(TrackingResultDetail @event)
    {
        _steps.Add(@event);
    }

    public abstract Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default);

    protected internal abstract Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default);
}
