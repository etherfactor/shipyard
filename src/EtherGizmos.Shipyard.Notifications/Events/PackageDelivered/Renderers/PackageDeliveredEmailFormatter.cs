using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class PackageDeliveredEmailFormatter
    : EmailNotificationChannelFormatter<ImmediateSchedule, PackageDeliveredEvent>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public PackageDeliveredEmailFormatter(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public override EmailEnvelope Format(
        Notification notification,
        PackageDeliveredEvent model)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        var userId = new Guid(notification.NotificationSubscription.UserId);
        var user = userRepo.Data.Single(e => e.Id == userId);

        var message = new EmailMessage()
        {
            Subject = "Package Delivered",
            To = [new(user.EmailAddress ?? throw new InvalidOperationException())],
            HtmlBody = "",
        };

        return new(message);
    }
}
