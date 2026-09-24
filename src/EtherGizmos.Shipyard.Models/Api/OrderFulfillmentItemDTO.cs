namespace EtherGizmos.Shipyard.Api;

public class OrderFulfillmentItemDTO
{
    public int OrderItemId { get; set; }

    public OrderItemDTO? OrderItem { get; set; }

    public decimal ExpectedQuantity { get; set; }

    public decimal? ReceivedQuantity { get; set; }
}
