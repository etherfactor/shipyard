using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;

namespace EtherGizmos.Shipyard.Database;

public class AppNotification : IEntity
{
    public virtual long Id { get; set; }

    public virtual Guid EventId { get; set; }

    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public virtual DateTimeOffset? SentAt { get; set; }

    public virtual long NotificationSubscriptionId { get; set; }

    public virtual AppNotificationSubscription NotificationSubscription { get; set; } = null!;

    public virtual bool IsDerived { get; set; }

    public virtual string PayloadType { get; set; } = null!;

    public virtual string Payload { get; set; } = null!;

    public virtual NotificationStatusType Status { get; set; }

    public virtual IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    public virtual int AttemptCount { get; set; }

    public virtual DateTimeOffset? LastAttemptAt { get; set; }

    public virtual string? LastError { get; set; }

    public virtual Guid? LockId { get; set; }

    public virtual string? LockedBy { get; set; }

    public virtual DateTimeOffset? LockedUntil { get; set; }
}
