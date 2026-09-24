using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class OrderItem : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual int OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual int Sequence { get; set; }

    public virtual string Description { get; set; } = null!;

    public virtual int Quantity { get; set; }

    public virtual string? QuantityUnits { get; set; }

    public virtual decimal? UnitPrice { get; set; }

    public virtual decimal? TotalPrice { get; set; }
}
