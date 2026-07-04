using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Services;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class CarrierUnknownStatusEmailFormatter
    : EmailNotificationChannelFormatter<ImmediateSchedule, CarrierUnknownStatusEvent>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CarrierUnknownStatusEmailFormatter(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory.AsUnfiltered();
    }

    public override EmailEnvelope Format(
        Notification notification,
        CarrierUnknownStatusEvent model)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        var userId = new Guid(notification.NotificationSubscription.UserId);
        var user = userRepo.Data.Single(e => e.Id == userId);

        var subject = model.Title;

        var html = LiquidMjmlRenderer.Render(
            "CarrierUnknownStatus.Templates.CarrierUnknownStatus",
            new
            {
                subject = subject,
                name = user.GivenName ?? user.Username,
                carrierId = model.CarrierId,
                carrierName = model.CarrierName,
                observedAt = model.ObservedAt,
                statusText = model.StatusText,
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
