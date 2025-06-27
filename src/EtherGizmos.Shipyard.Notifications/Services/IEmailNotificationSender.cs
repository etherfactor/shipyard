namespace EtherGizmos.Shipyard.Notifications.Services;

public interface IEmailNotificationSender<in TEvent> : INotificationSender<TEvent>;
