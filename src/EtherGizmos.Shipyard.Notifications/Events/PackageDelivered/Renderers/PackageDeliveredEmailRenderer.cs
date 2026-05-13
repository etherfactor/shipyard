using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;

namespace EtherGizmos.Shipyard.Events.PackageDelivered.Renderers;

public class PackageDeliveredEmailRenderer : EmailNotificationChannelFormatter<ImmediateSchedule, PackageDeliveredEvent>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public PackageDeliveredEmailRenderer(
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
