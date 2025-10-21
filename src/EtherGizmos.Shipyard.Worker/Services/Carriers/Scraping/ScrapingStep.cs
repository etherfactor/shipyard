using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public abstract class ScrapingStep
{
    private readonly IList<TrackingResultDetail> _steps = [];
    private readonly IList<TrackingResultArtifact> _artifacts = [];
    private DateTimeOffset? _eta;

    internal int Index { get; set; }

    internal ILogger Logger { get; set; } = NullLogger.Instance;

    internal DateTimeOffset? Eta => _eta;

    internal IReadOnlyList<TrackingResultDetail> Updates => _steps.AsReadOnly();

    internal IReadOnlyList<TrackingResultArtifact> Artifacts => _artifacts.AsReadOnly();

    protected void SetEta(DateTimeOffset eta)
    {
        _eta = eta;
    }

    protected void AddEvent(TrackingResultDetail @event)
    {
        _steps.Add(@event);
    }

    protected void AddArtifact(TrackingResultArtifact artifact)
    {
        _artifacts.Add(artifact);
    }

    public abstract Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default);

    protected internal abstract Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default);
}
