namespace EtherGizmos.Shipyard.Api;

public class OrderItemDTO
{
    public string Description { get; set; } = null!;

    public int Quantity { get; set; }

    public string? QuantityUnits { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }
}
