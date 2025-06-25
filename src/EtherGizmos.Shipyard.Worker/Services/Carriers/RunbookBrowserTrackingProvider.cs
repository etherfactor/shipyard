using EtherGizmos.Shipyard.Database.Services;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Models.Database.Enums;
using EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;
using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using Microsoft.EntityFrameworkCore;
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

        var carrier = await carrierRepo.Data.SingleAsync(e => e.Slug == _slug, cancellationToken: cancellationToken);

        var runbookRaw = carrier
            .Steps
            .Select(e => e.Payload
                .Prepend(new("stepType", e.StepType))
                .ToDictionary())
            .ToList();

        var runbookJson = JsonSerializer.Serialize(runbookRaw, _jsonOptions);
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
