using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Api;
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

    public int? OrderFulfillmentId { get; set; }

    public OrderFulfillment? OrderFulfillment { get; set; }

    public virtual DateTimeOffset? EstimatedDeliveryAt { get; set; }

    public virtual DateTimeOffset LastPollAt { get; set; }

    public virtual DateTimeOffset NextPollAt { get; set; }

    public virtual int LastTrackingStatusTypeId { get; set; }

    public virtual TrackingStatusType LastTrackingStatusType { get; set; } = null!;

    public virtual bool IsDelivered { get; set; }

    public DateTimeOffset? ReceivedAt { get; set; }

    public Guid? ReceivedByUserId { get; set; }

    public UserDTO? ReceivedByUser { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }

    public Guid? ResolvedByUserId { get; set; }

    public UserDTO? ResolvedByUser { get; set; }

    public string? ResolvedReason { get; set; }

    public List<PackageItem> PackageItems { get; set; } = [];

    public virtual List<TrackingUpdate> TrackingUpdates { get; set; } = [];

    public virtual int? GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Guid SecurableId { get; set; }

    public virtual Securable Securable { get; set; } = new() { Type = SecurableType.Package };
}
