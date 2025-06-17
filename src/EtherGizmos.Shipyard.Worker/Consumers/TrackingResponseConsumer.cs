using EtherGizmos.Messaging.Abstractions;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.Shipyard.Worker.Consumers;

public class TrackingResponseConsumer : IMessageConsumer<TrackingResponse>
{
    private readonly ILogger _logger;

    public TrackingResponseConsumer(
        ILogger<TrackingResponseConsumer> logger)
    {
        _logger = logger;
    }

    public Task ConsumeAsync(
        IMessageContext<TrackingResponse> context)
    {
        _logger.LogInformation("Received response message {@Message}", context.Message);

        return Task.CompletedTask;
    }
}

public record TrackingResponse
{
    public int PackageId { get; init; }

    public string Status { get; init; } = null!;

    public IReadOnlyList<TrackingResponseDetail> Details { get; init; } = [];
}

public record TrackingResponseDetail
{
    public DateTimeOffset Timestamp { get; init; }

    public string Status { get; init; } = null!;

    public string? Location { get; init; }

    public string? Description { get; init; }
}
