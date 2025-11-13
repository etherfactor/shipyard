using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class AclEntry : IEntity
{
    public virtual int Id { get; set; }

    public virtual Guid PrincipalId { get; set; }

    public virtual Guid? SecurableId { get; set; }

    public virtual SecurableType? SecurableType { get; set; }

    public virtual int PermissionId { get; set; }

    public virtual PermissionGrantType PermissionGrantType { get; set; }
}
