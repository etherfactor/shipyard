using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class OrderFulfillment : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual int OrderId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual string? Sender { get; set; }

    public virtual OrderFulfillmentStatusType OrderFulfillmentStatusType { get; set; }

    public virtual DateOnly? ShippedOn { get; set; }

    public virtual DateTimeOffset? ReceivedAt { get; set; }

    public virtual Guid? ReceivedByUserId { get; set; }

    public virtual User? ReceivedByUser { get; set; }

    public virtual DateTimeOffset? ResolvedAt { get; set; }

    public virtual Guid? ResolvedByUserId { get; set; }

    public virtual User? ResolvedByUser { get; set; }

    public virtual string? ResolvedReason { get; set; }

    public virtual List<OrderFulfillmentItem> OrderFulfillmentItems { get; set; } = [];
}
