using EtherGizmos.Common.Utilities.Abstractions;

namespace EtherGizmos.Shipyard.Models.Database;

public class CarrierStatusRule : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual int CarrierId { get; set; }

    public virtual Carrier Carrier { get; set; } = null!;

    public virtual string Pattern { get; set; } = null!;

    public virtual int StatusTypeId { get; set; }

    public virtual StatusType StatusType { get; set; } = null!;

    public virtual int Priority { get; set; }

    public virtual bool IsActive { get; set; }
}
