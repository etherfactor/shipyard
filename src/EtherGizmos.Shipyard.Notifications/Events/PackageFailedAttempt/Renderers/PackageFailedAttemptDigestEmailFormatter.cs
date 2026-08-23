using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Services;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class PackageFailedAttemptDigestEmailFormatter
    : EmailNotificationChannelFormatter<DigestSchedule, Digest<PackageFailedAttemptEvent>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly INotificationUnsubscribeService _unsubscribeService;

    public PackageFailedAttemptDigestEmailFormatter(
        IUnitOfWorkFactory uowFactory,
        INotificationUnsubscribeService unsubscribeService)
    {
        _uowFactory = uowFactory.AsUnfiltered();
        _unsubscribeService = unsubscribeService;
    }

    public override EmailEnvelope Format(
        Notification notification,
        Digest<PackageFailedAttemptEvent> model)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        var userId = new Guid(notification.Subscription.UserId);
        var user = userRepo.Data.Single(e => e.Id == userId);

        if (user.EmailAddress is null)
            throw new InvalidOperationException(EmailConstants.UserLacksEmailExceptionMessage);

        var orderedNotifications = model.Notifications
            .OrderBy(e => e.Updates.OrderBy(u => u.OccurredAt).LastOrDefault()?.OccurredAt)
            .ToList();

        var subject = orderedNotifications.Count == 1
            ? "1 Failed Delivery Attempt"
            : $"{orderedNotifications.Count} Failed Delivery Attempts";

        var shipyardUrl = orderedNotifications.FirstOrDefault()?.ShipyardUrl;

        var unsubscribeKey = _unsubscribeService
            .GetUnsubscribeKeyAsync(notification.SubscriptionId)
            .GetAwaiter()
            .GetResult();

        var unsubscribeUrl = $"{shipyardUrl}/notifications/unsubscribe?key={Uri.EscapeDataString(unsubscribeKey)}";

        var html = LiquidMjmlRenderer.Render(
            "PackageFailedAttempt.Templates.PackageFailedAttemptDigest",
            new
            {
                subject = subject,
                name = user.GivenName ?? user.Username,
                startAt = model.StartAt,
                endAt = model.EndAt,
                notificationCount = orderedNotifications.Count,
                notifications = orderedNotifications.Select(e =>
                {
                    var updates = e.Updates.OrderBy(u => u.OccurredAt).ToList();
                    var lastUpdate = updates.LastOrDefault();

                    return new
                    {
                        packageId = e.PackageId,
                        carrierName = e.CarrierName,
                        trackingNumber = e.TrackingNumber,
                        trackingUrl = e.TrackingUrl,
                        contents = e.Contents,
                        failedAttemptAt = lastUpdate?.OccurredAt.ToString(),
                        details = lastUpdate?.Description,
                    };
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
