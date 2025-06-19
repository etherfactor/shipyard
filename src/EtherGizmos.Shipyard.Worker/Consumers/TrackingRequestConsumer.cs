using EtherGizmos.Messaging.Abstractions;
using EtherGizmos.Shipyard.Worker.Services.Carriers;
using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.Shipyard.Worker.Consumers;

public class TrackingRequestConsumer : IMessageConsumer<TrackingRequest>
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageSender _sender;

    public TrackingRequestConsumer(
        ILogger<TrackingRequestConsumer> logger,
        IServiceProvider serviceProvider,
        IMessageSender sender)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _sender = sender;
    }

    public async Task ConsumeAsync(
        IMessageContext<TrackingRequest> context)
    {
        using var scope = _serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        var message = context.Message;

        _logger.LogInformation("Received request message {@Message}", message);

        var client = provider.GetRequiredService<IBrowserClient>();
        var tracker = new UspsBrowserTrackingProvider(client);

        var result = await tracker.TrackAsync(message.TrackingNumber, context.CancellationToken);

        await _sender.SendAsync("tracking-poll-response", new TrackingResponse()
        {
            PackageId = message.PackageId,
            Status = "Delivered",
            Details = [],
        }, cancellationToken: context.CancellationToken);
    }
}

public record TrackingRequest
{
    public int PackageId { get; init; }

    public int CarrierId { get; init; }

    public string TrackingNumber { get; init; } = null!;
}
