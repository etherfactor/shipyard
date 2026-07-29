using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Services;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class CarrierUnknownStatusDigestEmailFormatter
    : EmailNotificationChannelFormatter<DigestSchedule, Digest<CarrierUnknownStatusEvent>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CarrierUnknownStatusDigestEmailFormatter(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory.AsUnfiltered();
    }

    public override EmailEnvelope Format(
        Notification notification,
        Digest<CarrierUnknownStatusEvent> model)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        var userId = new Guid(notification.Subscription.UserId);
        var user = userRepo.Data.Single(e => e.Id == userId);

        var orderedNotifications = model.Notifications
            .OrderBy(e => e.ObservedAt)
            .ToList();

        var subject = orderedNotifications.Count == 1
            ? "1 Unknown Carrier Status"
            : $"{orderedNotifications.Count} Unknown Carrier Statuses";

        var html = LiquidMjmlRenderer.Render(
            "CarrierUnknownStatus.Templates.CarrierUnknownStatusDigest",
            new
            {
                subject = subject,
                name = user.GivenName ?? user.Username,
                startAt = model.StartAt,
                endAt = model.EndAt,
                notificationCount = orderedNotifications.Count,
                notifications = orderedNotifications.Select(e => new
                {
                    carrierId = e.CarrierId,
                    carrierName = e.CarrierName,
                    observedAt = e.ObservedAt,
                    statusText = e.StatusText,
                }),
                shipyardUrl = orderedNotifications.FirstOrDefault()?.ShipyardUrl,
                unsubscribeKey = "invalid",
            });

        var message = new EmailMessage()
        {
            Subject = subject,
            From = new("shipyard@localhost"),
            To = [new(user.EmailAddress ?? "test@domain.com")],
            HtmlBody = html,
        };

        return new(message);
    }
}
