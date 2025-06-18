using EtherGizmos.Shipyard.Models.Database.Base;

namespace EtherGizmos.Shipyard.Models.Database;

public class Package : Auditable
{
    public virtual int Id { get; set; }

    public virtual int CarrierId { get; set; }

    public virtual Carrier Carrier { get; set; } = null!;

    public virtual string TrackingNumber { get; set; } = null!;

    public virtual string? Contents { get; set; }

    public virtual int LastStatusTypeId { get; set; }

    public virtual ShipmentStatusType LastStatusType { get; set; } = null!;

    public virtual bool IsDelivered { get; set; }
}
