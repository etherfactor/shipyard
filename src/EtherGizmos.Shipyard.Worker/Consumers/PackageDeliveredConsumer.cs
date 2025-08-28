using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Messages;
using EtherGizmos.Shipyard.Models;
using EtherGizmos.Shipyard.Services;
using Microsoft.Extensions.Options;

namespace EtherGizmos.Shipyard.Worker.Consumers;

public class PackageDeliveredConsumer : IMessageConsumer<PackageDelivered>
{
    private readonly IOptionsMonitor<NotificationOptions> _notificationOptions;
    private readonly IEmailNotificationSender<PackageDeliveredEvent> _emailSender;

    public PackageDeliveredConsumer(
        IOptionsMonitor<NotificationOptions> notificationOptions,
        IEmailNotificationSender<PackageDeliveredEvent> emailSender)
    {
        _notificationOptions = notificationOptions;
        _emailSender = emailSender;
    }

    public async Task ConsumeAsync(
        IMessageContext<PackageDelivered> context)
    {
        var options = _notificationOptions.CurrentValue;

        if (!options.IsEnabled)
            return;

        if (!options.Email.IsEnabled)
            return;

        var message = context.Message;

        await _emailSender.NotifyAsync(new PackageDeliveredEvent()
        {
            PackageId = message.PackageId,
            CarrierId = message.CarrierId,
            CarrierName = message.CarrierName,
            TrackingNumber = message.TrackingNumber,
            Contents = message.Contents,
            OccurredAt = message.OccurredAt,
            Location = message.Location,
            Description = message.Description,
            EstimatedDeliveryAt = message.EstimatedDeliveryAt,
        }, cancellationToken: context.CancellationToken);
    }
}
