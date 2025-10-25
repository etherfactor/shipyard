using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Messages;
using EtherGizmos.Shipyard.Worker.Services.Carriers;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.Shipyard.Worker.Consumers;

public class TrackingRequestConsumer : IMessageConsumer<TrackingRequest>
{
    private readonly ILogger _logger;
    private readonly ITrackingProviderFactory _trackingProviderFactory;
    private readonly ITeeStreamScopeFactory _teeScopeFactory;
    private readonly IArtifactWriter _artifactWriter;
    private readonly IMessageSender _sender;

    public TrackingRequestConsumer(
        ILogger<TrackingRequestConsumer> logger,
        ITrackingProviderFactory trackingProviderFactory,
        ITeeStreamScopeFactory teeScopeFactory,
        IArtifactWriter artifactWriter,
        IMessageSender sender)
    {
        _logger = logger;
        _trackingProviderFactory = trackingProviderFactory;
        _teeScopeFactory = teeScopeFactory;
        _artifactWriter = artifactWriter;
        _sender = sender;
    }

    public async Task ConsumeAsync(
        IMessageContext<TrackingRequest> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received request message {@Message}", message);

        var ndjson = new MemoryStream();
        using var tee = _teeScopeFactory.Begin(ndjson);

        using var tracker = _trackingProviderFactory.CreateProvider(message.CarrierId, message.ExecutionId);

        var started = DateTimeOffset.UtcNow;
        try
        {
            var result = await tracker.TrackAsync(message.TrackingNumber, context.CancellationToken);

            tee.Dispose();
            ndjson.Position = 0;
            var ndjsonDesc = await _artifactWriter.WriteForRunAsync(message.ExecutionId, ArtifactFormat.NdJson, $"log", ndjson, cancellationToken: context.CancellationToken);

            _logger.LogInformation("Created NdJson artifact {ArtifactUri}", ndjsonDesc);

            var ended = DateTimeOffset.UtcNow;
            await _sender.SendAsync("tracking-poll-response", new TrackingResponse()
            {
                ExecutionId = message.ExecutionId,
                IsSuccess = true,
                StartedAt = started,
                CompletedAt = ended,
                PackageId = message.PackageId,
                EstimatedDeliveryAt = result.EstimatedDeliveryAt,
                Details = [.. result.Details.Select(e => new TrackingResponseDetail()
                {
                    OccurredAt = e.OccurredAt,
                    StatusTypeId = e.StatusTypeId,
                    Location = e.Location,
                    Description = e.Description,
                })],
                Artifacts = [.. result.Artifacts.Select(e => new TrackingResponseArtifact()
                {
                    Uri = e.Uri,
                    ContentType = e.ContentType,
                    Bytes = e.Bytes,
                    StepIndex = e.StepIndex,
                }), new()
                {
                    Uri = ndjsonDesc.Uri,
                    ContentType = ndjsonDesc.ContentType,
                    Bytes = ndjsonDesc.Bytes,
                    StepIndex = null,
                }],
            }, cancellationToken: context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process the tracking request");

            tee.Dispose();
            ndjson.Position = 0;
            var ndjsonDesc = await _artifactWriter.WriteForRunAsync(message.ExecutionId, ArtifactFormat.NdJson, $"log", ndjson, cancellationToken: context.CancellationToken);

            _logger.LogInformation("Created NdJson artifact {ArtifactUri}", ndjsonDesc);

            var ended = DateTimeOffset.UtcNow;
            await _sender.SendAsync("tracking-poll-response", new TrackingResponse()
            {
                ExecutionId = message.ExecutionId,
                IsSuccess = false,
                StartedAt = started,
                CompletedAt = ended,
                PackageId = message.PackageId,
                EstimatedDeliveryAt = null,
                Details = [],
                Artifacts = [new()
                {
                    Uri = ndjsonDesc.Uri,
                    ContentType = ndjsonDesc.ContentType,
                    Bytes = ndjsonDesc.Bytes,
                    StepIndex = null,
                }],
            }, cancellationToken: context.CancellationToken);
        }
    }
}
