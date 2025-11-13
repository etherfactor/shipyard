using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class Principal : IEntity
{
    public virtual Guid Id { get; set; }

    public virtual PrincipalType Type { get; init; }

    public virtual List<AclEntry> AclEntries { get; set; } = [];
}
