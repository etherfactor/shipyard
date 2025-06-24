using EtherGizmos.Shipyard.Database.Services;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Models.Database.Enums;
using EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;
using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

internal class RunbookBrowserTrackingProvider : ITrackingProvider
{
    private static readonly JsonSerializerOptions _jsonOptions;

    private readonly IServiceProvider _serviceProvider;
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBrowserClient _browserClient;
    private readonly string _slug;

    static RunbookBrowserTrackingProvider()
    {
        _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        _jsonOptions.Converters.Add(new ScrapingStepConverter());
    }

    public RunbookBrowserTrackingProvider(
        IServiceProvider serviceProvider,
        string slug)
    {
        _serviceProvider = serviceProvider;
        _uowFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
        _browserClient = serviceProvider.GetRequiredService<IBrowserClient>();
        _slug = slug;
    }

    public async Task<TrackingResult> TrackAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();
        var carrierRepo = uow.Repository<Carrier>();

        var runbookJson = """
            [
                {
                    "type": "Navigate",
                    "url": "https://tools.usps.com/go/TrackAction?tLabels={trackingNumber}"
                },
                {
                    "type": "WaitFor",
                    "selector": "span.tracking-number"
                },
                {
                    "type": "Click",
                    "selector": "div.toggle-history-container"
                },
                {
                    "type": "WaitFor",
                    "selector": "span.tracking-number"
                },
                {
                    "type": "Extract",
                    "selector": "strong.date",
                    "var" :"etaDay",
                    "trim": true
                },
                {
                    "type": "Extract",
                    "selector": "span.month_year > span:first-child",
                    "var": "etaMonth",
                    "trim": true
                },
                {
                    "type": "Extract",
                    "selector": "span.month_year",
                    "var": "etaYear",
                    "trim": true
                },
                {
                    "type": "Extract",
                    "selector": "strong.time",
                    "var": "etaTime",
                    "trim": true
                },
                {
                    "type": "Set",
                    "var": "estimatedAt",
                    "value": "{etaMonth} {etaDay}, {etaYear} {etaTime}",
                    "trim": true
                },
                {
                    "type": "ExtractList",
                    "selector": "div.tb-step",
                    "var": "details",
                    "steps": [
                        {
                            "type": "extract",
                            "selector": ".tb-status-detail",
                            "var": "description",
                            "trim": true
                        },
                        {
                            "type": "replace",
                            "var": "description",
                            "from": "&nbsp;",
                            "to": " ",
                            "trim": true
                        },
                        {
                            "type": "extract",
                            "selector": ".tb-location",
                            "var": "location",
                            "trim": true
                        },
                        {
                            "type": "replace",
                            "var": "location",
                            "from": "&nbsp;",
                            "to": " ",
                            "trim": true
                        },
                        {
                            "type": "extract",
                            "selector": ".tb-date",
                            "var": "occurredAt",
                            "trim": true
                        }
                    ]
                },
                {
                    "type": "Return",
                    "name": "estimatedAt",
                    "var": "estimatedAt"
                },
                {
                    "type": "Return",
                    "name": "details",
                    "var": "details"
                }
            ]
            """;

        var runbook = JsonSerializer.Deserialize<List<ScrapingStep>>(runbookJson, _jsonOptions) ?? [];

        var variables = new Dictionary<string, object>()
        {
            { "trackingNumber", trackingNumber },
            { "entryUrl", $"https://tools.usps.com/go/TrackAction?tLabels={HttpUtility.UrlEncode(trackingNumber)}" },
        };
        var results = new Dictionary<string, object>();
        foreach (var step in runbook)
        {
            await step.Apply(_browserClient, variables, results, cancellationToken);
        }

        var estimatedAt = results.TryGetValue("estimatedAt", out object? estimatedAtObj)
            ? estimatedAtObj is string estimatedAtStr
            ? DateTimeOffset.TryParse(estimatedAtStr, out var estimatedAtDt)
            ? estimatedAtDt as DateTimeOffset?
            : null : null : null;

        var details = results.TryGetValue("details", out object? detailsObj)
            ? detailsObj is List<object> detailsList
            ? detailsList.OfType<IDictionary<string, object>>().Cast<IDictionary<string, object>>().Select(subResults =>
            {
                var occurredAt = subResults.TryGetValue("occurredAt", out object? occurredAtObj)
                    ? occurredAtObj is string occurredAtStr
                    ? DateTimeOffset.TryParse(occurredAtStr, out var occurredAtDt)
                    ? occurredAtDt
                    : default : default : default;

                var location = subResults.TryGetValue("location", out object? locationObj)
                    ? locationObj is string locationStr
                    ? locationStr
                    : null : null;

                var description = subResults.TryGetValue("description", out object? descriptionObj)
                    ? descriptionObj is string descriptionStr
                    ? descriptionStr
                    : null : null;

                return new TrackingResultDetail()
                {
                    OccurredAt = occurredAt,
                    StatusTypeId = StatusTypeId.Unknown,
                    Location = NullIfEmpty(location),
                    Description = NullIfEmpty(description),
                };
            })
            : [] : [];

        details = details
            .Where(e => e.OccurredAt != DateTimeOffset.MinValue);

        var result = new TrackingResult()
        {
            TrackingNumber = trackingNumber,
            EstimatedDeliveryAt = estimatedAt,
            Details = [.. details],
        };

        return result;
    }

    private string? NullIfEmpty(string? input)
    {
        return !string.IsNullOrWhiteSpace(input) ? input : null;
    }
}
