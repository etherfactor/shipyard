using EtherGizmos.Shipyard.Models;

namespace EtherGizmos.Shipyard.Services;

public interface IEmailNotificationSender<in TEvent> : INotificationSender<TEvent>
    where TEvent : NotificationEvent;
