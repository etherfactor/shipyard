using EtherGizmos.Shipyard.Models;

namespace EtherGizmos.Shipyard.Services;

internal class NullEmailNotificationSender<TEvent> : IEmailNotificationSender<TEvent>
    where TEvent : NotificationEvent
{
    public Task NotifyAsync(TEvent notification, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
