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
    private readonly INotificationUnsubscribeService _unsubscribeService;

    public CarrierUnknownStatusEmailFormatter(
        IUnitOfWorkFactory uowFactory,
        INotificationUnsubscribeService unsubscribeService)
    {
        _uowFactory = uowFactory.AsUnfiltered();
        _unsubscribeService = unsubscribeService;
    }

    public override EmailEnvelope Format(
        Notification notification,
        CarrierUnknownStatusEvent model)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        var userId = new Guid(notification.Subscription.UserId);
        var user = userRepo.Data.Single(e => e.Id == userId);

        if (user.EmailAddress is null)
            throw new InvalidOperationException(EmailConstants.UserLacksEmailExceptionMessage);

        var subject = model.Title;

        var shipyardUrl = model.ShipyardUrl;

        var unsubscribeKey = _unsubscribeService
            .GetUnsubscribeKeyAsync(notification.SubscriptionId)
            .GetAwaiter()
            .GetResult();

        var unsubscribeUrl = $"{shipyardUrl}/notifications/unsubscribe?key={Uri.EscapeDataString(unsubscribeKey)}";

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
