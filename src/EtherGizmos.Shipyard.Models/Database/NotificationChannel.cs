using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class NotificationChannel : IEntity
{
    public virtual string Id { get; set; } = null!;

    public virtual string Name { get; set; } = null!;

    public virtual IDictionary<string, object?> ConfigSchema { get; set; } = new Dictionary<string, object?>();
}
