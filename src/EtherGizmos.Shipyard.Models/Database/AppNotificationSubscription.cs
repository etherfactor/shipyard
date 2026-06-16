using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class AppNotificationSubscription : IEntity
{
    public virtual long Id { get; set; }

    public virtual string UserId { get; set; } = null!;

    public virtual string EventType { get; set; } = null!;

    public virtual AppNotificationEvent Event { get; set; } = null!;

    public virtual string ChannelKey { get; set; } = null!;

    public virtual AppNotificationChannel Channel { get; set; } = null!;

    public virtual string ChannelConfigRaw { get; set; } = null!;

    public virtual string ScheduleType { get; set; } = null!;

    public virtual AppNotificationSchedule Schedule { get; set; } = null!;

    public virtual string ScheduleConfigRaw { get; set; } = null!;

    public virtual bool IsEnabled { get; set; }

    public virtual DateTimeOffset? LastNotificationAt { get; set; }

    public virtual DateTimeOffset? NextNotificationAt { get; set; }
}
