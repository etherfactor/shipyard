using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Services;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class PackageOutForDeliveryEmailFormatter
    : EmailNotificationChannelFormatter<ImmediateSchedule, PackageOutForDeliveryEvent>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly INotificationUnsubscribeService _unsubscribeService;

    public PackageOutForDeliveryEmailFormatter(
        IUnitOfWorkFactory uowFactory,
        INotificationUnsubscribeService unsubscribeService)
    {
        _uowFactory = uowFactory.AsUnfiltered();
        _unsubscribeService = unsubscribeService;
    }

    public override EmailEnvelope Format(
        Notification notification,
        PackageOutForDeliveryEvent model)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        var userId = new Guid(notification.Subscription.UserId);
        var user = userRepo.Data.Single(e => e.Id == userId);

        if (user.EmailAddress is null)
            throw new InvalidOperationException(EmailConstants.UserLacksEmailExceptionMessage);

        var updates = model.Updates.OrderBy(e => e.OccurredAt);

        var subject = model.Title;

        var shipyardUrl = model.ShipyardUrl;

        var unsubscribeKey = _unsubscribeService
            .GetUnsubscribeKeyAsync(notification.SubscriptionId)
            .GetAwaiter()
            .GetResult();

        var unsubscribeUrl = $"{shipyardUrl}/notifications/unsubscribe?key={Uri.EscapeDataString(unsubscribeKey)}";

        var html = LiquidMjmlRenderer.Render(
            "PackageOutForDelivery.Templates.PackageOutForDelivery",
            new
            {
                subject = subject,
                trackingNumber = model.TrackingNumber,
                trackingUrl = model.TrackingUrl,
                name = user.GivenName ?? user.Username,
                carrierName = model.CarrierName,
                outForDeliveryAt = updates.LastOrDefault()?.OccurredAt.ToString(),
                details = updates.LastOrDefault()?.Description,
                contents = model.Contents,
                packageId = model.PackageId,
                updates = updates.Reverse().Take(5).Select(e => new
                {
                    occurredAt = e.OccurredAt,
                    details = e.Description,
                }),
                shipyardUrl = shipyardUrl,
                unsubscribeUrl = unsubscribeUrl,
            });

        var message = new EmailMessage()
        {
            Subject = subject,
            To = [new(user.EmailAddress)],
            HtmlBody = html,
        };

        return new(message);
    }
}
