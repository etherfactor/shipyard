using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Extensions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Messages;
using EtherGizmos.Shipyard.Services.Carriers;
using Microsoft.Extensions.Logging;

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
        var message = context.Message;
        _logger.LogInformation("Received request message {@Message}", message);

        using var dotnet = _logger.BeginScope("Language", "Dotnet");

        var ndjson = new MemoryStream();
        using var tee = _teeScopeFactory.Begin(ndjson);

        using var tracker = _trackingProviderFactory.CreateProvider(message.CarrierId, message.ExecutionId);

        var started = DateTimeOffset.UtcNow;
        try
        {
            var result = await tracker.TrackAsync(message.TrackingNumber, context.CancellationToken);

            //Have to post results back
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process the tracking request");

            //Have to post results back
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
