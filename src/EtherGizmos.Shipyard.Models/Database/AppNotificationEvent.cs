using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class AppNotificationEvent : IEntity
{
    public virtual string Id { get; set; } = null!;

    public virtual string Name { get; set; } = null!;

    public virtual List<AppNotificationEventChannelSchedule> Supports { get; set; } = [];
}
