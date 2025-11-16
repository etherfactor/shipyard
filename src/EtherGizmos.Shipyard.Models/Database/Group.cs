using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class Group : IEntity
{
    public virtual int Id { get; set; }

    public virtual string Name { get; set; } = null!;

    public virtual string? Description { get; set; }

    public virtual Guid? SystemId { get; set; }

    public virtual List<User> Users { get; set; } = [];

    public virtual Guid SecurableId { get; set; }

    public virtual Securable Securable { get; set; } = new() { Type = SecurableType.Group };
}
