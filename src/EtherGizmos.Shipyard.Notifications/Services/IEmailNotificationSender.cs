using EtherGizmos.Shipyard.Notifications.Models;

namespace EtherGizmos.Shipyard.Notifications.Services;

public interface IEmailNotificationSender<in TEvent> : INotificationSender<TEvent>
    where TEvent : NotificationEvent;
