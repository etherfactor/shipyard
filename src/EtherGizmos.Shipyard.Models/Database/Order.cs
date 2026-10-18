using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class Order : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual string? Source { get; set; }

    public virtual string? OrderNumber { get; set; }

    public virtual DateOnly? OrderedOn { get; set; }

    public virtual decimal? TotalPrice { get; set; }

    public virtual string? OrderUrl { get; set; }

    public virtual DateOnly? ExpectedShipStartOn { get; set; }

    public virtual DateOnly? ExpectedShipEndOn { get; set; }

    public virtual List<OrderItem> OrderItems { get; set; } = [];

    public virtual List<OrderFulfillment> OrderFulfillments { get; set; } = [];
}
