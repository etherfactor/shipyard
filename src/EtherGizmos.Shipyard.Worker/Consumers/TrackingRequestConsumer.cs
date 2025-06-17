using EtherGizmos.Messaging.Abstractions;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.Shipyard.Worker.Consumers;

public class TrackingRequestConsumer : IMessageConsumer<TrackingRequest>
{
    private readonly ILogger _logger;
    private readonly IMessageSender _sender;

    public TrackingRequestConsumer(
        ILogger<TrackingRequestConsumer> logger,
        IMessageSender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task ConsumeAsync(
        IMessageContext<TrackingRequest> context)
    {
        _logger.LogInformation("Received request message {@Message}", context.Message);

        await _sender.SendAsync("tracking-poll-response", new TrackingResponse()
        {
            PackageId = context.Message.PackageId,
            Status = "Delivered",
            Details = [],
        }, cancellationToken: context.CancellationToken);
    }
}

public record TrackingRequest
{
    public int PackageId { get; init; }

    public string CarrierId { get; init; } = null!;

    public string ReferenceNumber { get; init; } = null!;
}
