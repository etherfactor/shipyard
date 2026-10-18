namespace EtherGizmos.Shipyard.Api;

public class PackageItemDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int PackageId { get; set; }

    public PackageDTO? Package { get; set; }

    public int? OrderFulfillmentItemId { get; set; }

    public OrderFulfillmentItemDTO? OrderFulfillmentItem { get; set; }

    public string Description { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string? QuantityUnits { get; set; }
}
