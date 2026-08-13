using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Services;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class PackageUnknownStatusDigestEmailFormatter
    : EmailNotificationChannelFormatter<DigestSchedule, Digest<PackageUnknownStatusEvent>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public PackageUnknownStatusDigestEmailFormatter(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory.AsUnfiltered();
    }

    public override EmailEnvelope Format(
        Notification notification,
        Digest<PackageUnknownStatusEvent> model)
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
            ? "1 Unknown Package Status"
            : $"{orderedNotifications.Count} Unknown Package Statuses";

        var html = LiquidMjmlRenderer.Render(
            "PackageUnknownStatus.Templates.PackageUnknownStatusDigest",
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
                        carrierId = e.CarrierId,
                        carrierName = e.CarrierName,
                        trackingNumber = e.TrackingNumber,
                        trackingUrl = e.TrackingUrl,
                        contents = e.Contents,
                        unknownStatusAt = lastUpdate?.OccurredAt.ToString(),
                        details = lastUpdate?.Description,
                    };
                }),
                shipyardUrl = orderedNotifications.FirstOrDefault()?.ShipyardUrl,
                unsubscribeKey = "invalid",
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
