using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Api.Enums;
using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Messages;
using EtherGizmos.Shipyard.Services.Carriers;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Consumers;

public class TrackingRequestConsumer : IMessageConsumer<TrackingRequest>
{
    private readonly ILogger _logger;
    private readonly IArtifactSender _artifactSender;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITeeStreamScopeFactory _teeScopeFactory;
    private readonly ITrackingProviderFactory _trackingProviderFactory;

    public TrackingRequestConsumer(
        ILogger<TrackingRequestConsumer> logger,
        IArtifactSender artifactSender,
        IHttpClientFactory httpClientFactory,
        ITeeStreamScopeFactory teeScopeFactory,
        ITrackingProviderFactory trackingProviderFactory)
    {
        _logger = logger;
        _artifactSender = artifactSender;
        _httpClientFactory = httpClientFactory;
        _teeScopeFactory = teeScopeFactory;
        _trackingProviderFactory = trackingProviderFactory;
    }

    public async Task ConsumeAsync(
        IMessageContext<TrackingRequest> context)
    {
        using var client = _httpClientFactory.CreateClient("API");

        var message = context.Message;
        _logger.LogInformation("Received request message {@Message}", message);

        using var dotnet = _logger.BeginScope("Language", "Dotnet");

        var ndjson = new MemoryStream();
        using var tee = _teeScopeFactory.Begin(ndjson);

        using var startResponse = await client.PatchAsJsonAsync(
            $"/api/v1/carrierExecutions({message.ExecutionId})",
            new
            {
                startedAt = DateTimeOffset.UtcNow,
                executionStatusType = "Running",
            }, cancellationToken: context.CancellationToken);

        startResponse.EnsureSuccessStatusCode();

        using var tracker = _trackingProviderFactory.CreateProvider(message.CarrierId, message.ExecutionId);

        var started = DateTimeOffset.UtcNow;
        try
        {
            var result = await tracker.TrackAsync(message.TrackingNumber, context.CancellationToken);

            var resultUpdates = result.Details
                .Select(e => new TrackingUpdateDTO()
                {
                    OccurredAt = e.OccurredAt,
                    StatusType = (StatusTypeDTO)e.StatusTypeId,
                    Location = e.Location,
                    Description = e.Description,
                    PackageId = message.PackageId,
                });

            var package = (await client.GetFromJsonAsync<PackageDTO>(
                $"/api/v1/packages({message.PackageId})?$expand=trackingUpdates",
                JsonSerializerOptions.App,
                cancellationToken: context.CancellationToken))!;

            var updates = package.TrackingUpdates
                .GroupBy(e => e.OccurredAt)
                .ToDictionary(e => e.Key, e => e.First());

            var toCreateOrUpdate = new List<TrackingUpdateDTO>();
            foreach (var update in resultUpdates)
            {
                if (updates.TryAdd(update.OccurredAt, update))
                {
                    toCreateOrUpdate.Add(update);
                }

                var current = updates[update.OccurredAt];
                update.Id = current.Id;

                if (current.StatusType != update.StatusType
                    || current.Location != update.Location
                    || current.Description != update.Description)
                {
                    toCreateOrUpdate.Add(update);
                }
            }

            foreach (var update in toCreateOrUpdate)
            {
                if (update.Id == 0)
                {
                    await client.PostAsJsonAsync(
                        $"/api/v1/trackingUpdates",
                        new
                        {
                            occurredAt = update.OccurredAt,
                            statusType = update.StatusType.ToString(),
                            location = update.Location,
                            description = update.Description,
                            packageId = update.PackageId,
                        }, cancellationToken: context.CancellationToken);
                }
                else
                {
                    await client.PatchAsJsonAsync(
                        $"/api/v1/trackingUpdates({update.Id})",
                        new
                        {
                            occurredAt = update.OccurredAt,
                            statusType = update.StatusType.ToString(),
                            location = update.Location,
                            description = update.Description,
                            packageId = update.PackageId,
                        }, cancellationToken: context.CancellationToken);
                }
            }

            var regenResponse = await client.PatchAsJsonAsync(
                $"/api/v1/packages({message.PackageId})",
                new { }, //Intentional no-op; updating the package causes it to recalculate the last status
                cancellationToken: context.CancellationToken);

            regenResponse.EnsureSuccessStatusCode();

            using var endResponse = await client.PatchAsJsonAsync(
                $"/api/v1/carrierExecutions({message.ExecutionId})",
                new
                {
                    completedAt = DateTimeOffset.UtcNow,
                    executionStatusType = "Successful",
                }, cancellationToken: context.CancellationToken);

            endResponse.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process the tracking request");

            using var endResponse = await client.PatchAsJsonAsync(
                $"/api/v1/carrierExecutions({message.ExecutionId})",
                new
                {
                    completedAt = DateTimeOffset.UtcNow,
                    executionStatusType = "Failed",
                }, cancellationToken: context.CancellationToken);

            endResponse.EnsureSuccessStatusCode();
        }
        finally
        {
            tee.Dispose();
            ndjson.Position = 0;

            await _artifactSender.SendAsync(
                message.ExecutionId,
                "application/x-ndjson",
                "log.ndjson",
                ndjson,
                cancellationToken: context.CancellationToken);
        }
    }
}
