using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class NotificationEventChannelSchedule : IEntity
{
    public virtual string NotificationEventId { get; set; } = null!;

    public virtual NotificationEvent NotificationEvent { get; set; } = null!;

    public virtual string NotificationChannelId { get; set; } = null!;

    public virtual NotificationChannel NotificationChannel { get; set; } = null!;

    public virtual string NotificationScheduleId { get; set; } = null!;

    public virtual NotificationSchedule NotificationSchedule { get; set; } = null!;
}
