using EtherGizmos.Messaging.Abstractions;
using EtherGizmos.Shipyard.Worker.Consumers;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.Shipyard.Worker.Services;

public class QueueTrackingRequestBackgroundService : PeriodicBackgroundService
{
    private const string CRON_EXPRESSION = "*/15 * * * * *";

    private readonly IMessageSender _sender;

    public QueueTrackingRequestBackgroundService(
        ILogger<QueueTrackingRequestBackgroundService> logger,
        IMessageSender sender)
        : base(CRON_EXPRESSION, logger)
    {
        _sender = sender;
    }

    protected override async Task ExecuteIterationAsync(CancellationToken stoppingToken)
    {
        await _sender.SendAsync("tracking-poll-request", new TrackingRequest()
        {
            PackageId = 1,
            CarrierId = "usps",
            ReferenceNumber = "123456789",
        }, cancellationToken: stoppingToken);
    }
}
