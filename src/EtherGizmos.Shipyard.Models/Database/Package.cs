using EtherGizmos.Shipyard.Utilities.Abstractions;

namespace EtherGizmos.Shipyard.Models.Database;

public class Package : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual int CarrierId { get; set; }

    public virtual Carrier Carrier { get; set; } = null!;

    public virtual string TrackingNumber { get; set; } = null!;

    public virtual string? Contents { get; set; }

    public virtual DateTimeOffset LastPollAt { get; set; }

    public virtual DateTimeOffset NextPollAt { get; set; }

    public virtual int LastStatusTypeId { get; set; }

    public virtual StatusType LastStatusType { get; set; } = null!;

    public virtual bool IsDelivered { get; set; }
}
