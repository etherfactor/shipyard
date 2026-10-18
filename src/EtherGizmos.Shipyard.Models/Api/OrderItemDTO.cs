namespace EtherGizmos.Shipyard.Api;

public class OrderItemDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int OrderId { get; set; }

    public OrderDTO? Order { get; set; }

    public string Description { get; set; } = null!;

    public int Quantity { get; set; }

    public string? QuantityUnits { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }
}
