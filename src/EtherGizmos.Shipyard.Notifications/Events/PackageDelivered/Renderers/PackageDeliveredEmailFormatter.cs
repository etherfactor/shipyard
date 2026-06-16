using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Services;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class PackageDeliveredEmailFormatter
    : EmailNotificationChannelFormatter<ImmediateSchedule, PackageDeliveredEvent>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public PackageDeliveredEmailFormatter(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory.AsUnfiltered();
    }

    public override EmailEnvelope Format(
        Notification notification,
        PackageDeliveredEvent model)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        var userId = new Guid(notification.NotificationSubscription.UserId);
        var user = userRepo.Data.Single(e => e.Id == userId);

        var updates = model.Updates.OrderBy(e => e.OccurredAt);

        var subject = model.Title;

        var html = LiquidMjmlRenderer.Render(
            "PackageDelivered.Templates.PackageDelivered",
            new
            {
                subject = subject,
                trackingNumber = model.TrackingNumber,
                trackingUrl = model.TrackingUrl,
                name = user.GivenName ?? user.Username,
                carrierName = model.CarrierName,
                deliveredAt = updates.LastOrDefault()?.OccurredAt.ToString(),
                details = updates.LastOrDefault()?.Description,
                contents = model.Contents,
                packageId = model.PackageId,
                updates = updates.Reverse().Take(5).Select(e => new
                {
                    occurredAt = e.OccurredAt,
                    details = e.Description,
                }),
                shipyardUrl = model.ShipyardUrl,
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
