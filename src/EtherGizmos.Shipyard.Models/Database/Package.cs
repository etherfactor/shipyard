using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class Package : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual int CarrierId { get; set; }

    public virtual Carrier Carrier { get; set; } = null!;

    public virtual string TrackingNumber { get; set; } = null!;

    public virtual bool IsItemized { get; set; }

    public virtual string? Contents { get; set; }

    public virtual int? OrderFulfillmentId { get; set; }

    public virtual OrderFulfillment? OrderFulfillment { get; set; }

    public virtual DateTimeOffset? EstimatedDeliveryAt { get; set; }

    public virtual DateTimeOffset LastPollAt { get; set; }

    public virtual DateTimeOffset NextPollAt { get; set; }

    public virtual int LastTrackingStatusTypeId { get; set; }

    public virtual TrackingStatusType LastTrackingStatusType { get; set; } = null!;

    public virtual bool IsDelivered { get; set; }

    public virtual DateTimeOffset? ReceivedAt { get; set; }

    public virtual Guid? ReceivedByUserId { get; set; }

    public virtual User? ReceivedByUser { get; set; }

    public virtual DateTimeOffset? ResolvedAt { get; set; }

    public virtual Guid? ResolvedByUserId { get; set; }

    public virtual User? ResolvedByUser { get; set; }

    public virtual string? ResolvedReason { get; set; }

    public virtual List<PackageItem> PackageItems { get; set; } = [];

    public virtual List<TrackingUpdate> TrackingUpdates { get; set; } = [];

    public virtual int? GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Guid SecurableId { get; set; }

    public virtual Securable Securable { get; set; } = new() { Type = SecurableType.Package };
}
