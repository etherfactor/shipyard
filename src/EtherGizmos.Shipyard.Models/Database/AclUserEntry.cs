using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class AclUserEntry
{
    public virtual int EntryId { get; set; }

    public virtual Guid PrincipalUserId { get; set; }

    public virtual User PrincipalUser { get; set; } = null!;

    public virtual Guid? SecurableId { get; set; }

    public virtual SecurableType? SecurableType { get; set; }

    public virtual int PermissionId { get; set; }

    public virtual PermissionGrantType PermissionGrantType { get; set; }
}
