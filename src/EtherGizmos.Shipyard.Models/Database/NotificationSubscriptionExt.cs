using EtherGizmos.Common.Models;

namespace EtherGizmos.Shipyard.Database;

public class NotificationSubscriptionExt : NotificationSubscription
{
    public virtual NotificationEvent Event { get; set; } = null!;

    public virtual NotificationChannel Channel { get; set; } = null!;

    public virtual NotificationSchedule Schedule { get; set; } = null!;
}
