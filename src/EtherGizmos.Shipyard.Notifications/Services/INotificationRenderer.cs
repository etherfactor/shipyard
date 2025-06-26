namespace EtherGizmos.Shipyard.Notifications.Services;

public interface INotificationRenderer<in TEvent> : ISmtpNotificationRenderer<TEvent>
    where TEvent : class;
