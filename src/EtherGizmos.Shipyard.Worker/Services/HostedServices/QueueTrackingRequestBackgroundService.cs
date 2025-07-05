using EtherGizmos.Common.Messaging.Abstractions;
using EtherGizmos.Shipyard.Database.Services;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Worker.Consumers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.Shipyard.Worker.Services.HostedServices;

public class QueueTrackingRequestBackgroundService : PeriodicBackgroundService
{
    private const string CRON_EXPRESSION = "*/60 * * * * *";

    private readonly IMessageSender _sender;

    public QueueTrackingRequestBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<QueueTrackingRequestBackgroundService> logger,
        IMessageSender sender)
        : base(CRON_EXPRESSION, serviceProvider, logger)
    {
        _sender = sender;
    }

    protected override async Task ExecuteIterationAsync(
        IServiceProvider provider,
        CancellationToken stoppingToken)
    {
        var uowFactory = provider.GetRequiredService<IUnitOfWorkFactory>();
        using var uow = uowFactory.Create();

        var packageRepo = uow.Repository<Package>();

        var ready = packageRepo.Data
            .Where(e => e.NextPollAt < DateTimeOffset.UtcNow)
            .Include(e => e.Carrier)
            .Include(e => e.LastStatusType)
            .AsAsyncEnumerable();

        await Parallel.ForEachAsync(ready, async (package, ct) =>
        {
            await _sender.SendAsync("tracking-poll-request", new TrackingRequest()
            {
                PackageId = package.Id,
                CarrierSlug = package.Carrier.Slug,
                TrackingNumber = package.TrackingNumber,
            }, cancellationToken: stoppingToken);

            package.LastPollAt = DateTimeOffset.UtcNow;
            package.NextPollAt = package.LastPollAt
                + TimeSpan.FromHours(6) * (double)package.LastStatusType.PollingFactor;
        });

        await uow.SaveChangesAsync(stoppingToken);
    }
}
