namespace EtherGizmos.Shipyard.Api;

public class PackageItemDTO
{
    public int? OrderFulfillmentItemId { get; set; }

    public OrderFulfillmentItemDTO? OrderFulfillmentItem { get; set; }

    public string Description { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string? QuantityUnits { get; set; }
}
