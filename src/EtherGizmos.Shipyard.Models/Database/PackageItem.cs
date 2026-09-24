using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class PackageItem : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual int PackageId { get; set; }

    public virtual Package Package { get; set; } = null!;

    public virtual int Sequence { get; set; }

    public virtual int? OrderFulfillmentItemId { get; set; }

    public virtual OrderFulfillmentItem? OrderFulfillmentItem { get; set; }

    public virtual string Description { get; set; } = null!;

    public virtual decimal Quantity { get; set; }

    public virtual string? QuantityUnits { get; set; }
}
