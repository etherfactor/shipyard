namespace EtherGizmos.Shipyard.Api;

public class OrderDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public string? Source { get; set; }

    public string? OrderNumber { get; set; }

    public DateOnly? OrderedOn { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? OrderUrl { get; set; }

    public DateOnly? ExpectedShipStartOn { get; set; }

    public DateOnly? ExpectedShipEndOn { get; set; }

    public List<OrderItemDTO> OrderItems { get; set; } = [];
}
