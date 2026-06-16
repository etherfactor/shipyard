using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class AppNotificationEventChannelSchedule : IEntity
{
    public virtual string NotificationEventId { get; set; } = null!;

    public virtual AppNotificationEvent NotificationEvent { get; set; } = null!;

    public virtual string NotificationChannelId { get; set; } = null!;

    public virtual AppNotificationChannel NotificationChannel { get; set; } = null!;

    public virtual string NotificationScheduleId { get; set; } = null!;

    public virtual AppNotificationSchedule NotificationSchedule { get; set; } = null!;
}
