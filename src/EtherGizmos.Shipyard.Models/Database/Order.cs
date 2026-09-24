using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class Order : Auditable, IEntity
{
    public int Id { get; set; }

    public string? Source { get; set; }

    public string? OrderNumber { get; set; }

    public DateOnly? OrderedOn { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? OrderUrl { get; set; }

    public DateOnly? ExpectedShipStartOn { get; set; }

    public DateOnly? ExpectedShipEndOn { get; set; }

    public List<OrderItem> OrderItems { get; set; } = [];

    public List<OrderFulfillment> OrderFulfillments { get; set; } = [];
}
