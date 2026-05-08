using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Extensions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Services.Carriers.Scraping;
using EtherGizmos.Shipyard.Services.WebDrivers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Services.Carriers;

internal class RunbookBrowserTrackingProvider : ITrackingProvider, IDisposable
{
    private static readonly JsonSerializerOptions _jsonOptions;

    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBrowserClient _browserClient;
    private readonly int _carrierId;
    private readonly int _executionId;

    private bool _disposed;

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
        int carrierId,
        int executionId)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<RunbookBrowserTrackingProvider>>();
        _uowFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>().AsUnfiltered();
        _browserClient = serviceProvider.GetRequiredService<IBrowserClient>();
        _carrierId = carrierId;
        _executionId = executionId;
    }

    public async Task<TrackingResult> TrackAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();
        var carrierRepo = uow.Repository<Carrier>();

        var carrier = await carrierRepo.Data.SingleAsync(e => e.Id == _carrierId, cancellationToken: cancellationToken);

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
        };
        var results = new Dictionary<string, object>();

        var runId = _executionId;
        var artifactWriter = _serviceProvider.GetRequiredService<IArtifactWriter>();
        var artifactSender = _serviceProvider.GetRequiredService<IArtifactSender>();

        var index = 0;
        ApplyServices(runbook);
        foreach (var step in runbook)
        {
            step.Index = ++index;
            using var stepmark = _logger.BeginScope("Step", step.Index);
            using (_logger.BeginScope("FLAG", "STEP_START"))
                _logger.LogInformation("[step={Step}] {StepName}", step.Index, step.StepName);

            var stopwatch = Stopwatch.StartNew();

            await step.Apply(_browserClient, variables, results, cancellationToken);

            var html = await _browserClient.GetHtmlAsync(cancellationToken);
            using var webp = await _browserClient.GetScreenshotAsync(cancellationToken);

            using var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            await artifactSender.SendAsync(runId, "text/html", $"page-{step.Index}.html", htmlStream, cancellationToken: cancellationToken);
            await artifactSender.SendAsync(runId, "image/webp", $"screenshot-{step.Index}.webp", webp, cancellationToken: cancellationToken);

            using (_logger.BeginScope("FLAG", "STEP_END"))
                _logger.LogInformation("[step={Step}] completed in {StepDuration}ms", step.Index, stopwatch.ElapsedMilliseconds);
        }

        var estimatedAt = results.TryGetValue("estimatedAt", out object? estimatedAtObj)
            ? estimatedAtObj is string estimatedAtStr
            ? DateTimeOffset.TryParse(estimatedAtStr, out var estimatedAtDt)
            ? estimatedAtDt as DateTimeOffset?
            : null : null : null;

        estimatedAt = runbook
            .Select(e => e.Eta)
            .Where(e => e is not null).Min()
            ?? estimatedAt;

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

        details = details.Concat(runbook.SelectMany(e => e.Updates));

        details = details
            .Where(e => e.OccurredAt != DateTimeOffset.MinValue);

        var result = new TrackingResult()
        {
            TrackingNumber = trackingNumber,
            EstimatedDeliveryAt = estimatedAt,
            Details = [.. details],
            Artifacts = [],
        };

        return result;
    }

    private void ApplyServices(
        IEnumerable<ScrapingStep> steps)
    {
        foreach (var step in steps)
        {
            step.ServiceProvider = _serviceProvider;
            step.Logger = _logger;

            if (step is ExtractListStep extractStep)
            {
                ApplyServices(extractStep.Steps);
            }
        }
    }

    private string? NullIfEmpty(string? input)
    {
        return !string.IsNullOrWhiteSpace(input) ? input : null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _browserClient.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
