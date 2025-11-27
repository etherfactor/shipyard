using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class RoleUser : Auditable, IEntity
{
    public virtual int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
