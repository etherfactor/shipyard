using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class AclRole : IEntity
{
    public Guid PrincipalUserId { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public short IsGrant { get; set; }

    public PermissionGrantType PermissionGrantType { get; set; }
}
