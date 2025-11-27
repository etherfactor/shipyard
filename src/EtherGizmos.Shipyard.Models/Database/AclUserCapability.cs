using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class AclUserCapability
{
    public virtual Guid PrincipalUserId { get; set; }

    public virtual User PrincipalUser { get; set; } = null!;

    public virtual SecurableType SecurableType { get; set; }

    public virtual int PermissionId { get; set; }

    public short IsAllowed { get; set; }
}
