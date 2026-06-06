using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class NotificationEvent : IEntity
{
    public virtual string Id { get; set; } = null!;

    public virtual string Name { get; set; } = null!;

    public virtual List<NotificationEventChannelSchedule> Supports { get; set; } = [];
}
