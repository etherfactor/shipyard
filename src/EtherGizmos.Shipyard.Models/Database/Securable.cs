using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class Securable
{
    public virtual Guid Id { get; set; }

    public virtual SecurableType Type { get; init; }
}
