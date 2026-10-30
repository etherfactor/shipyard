using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class AclOrder
{
    public Guid PrincipalUserId { get; set; }

    public int OrderId { get; set; }

    public int PermissionId { get; set; }

    public short IsGrant { get; set; }

    public PermissionGrantType PermissionGrantType { get; set; }
}
