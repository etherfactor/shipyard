namespace EtherGizmos.Shipyard.Api;

public class OrderFulfillmentItemDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int OrderFulfillmentId { get; set; }

    public OrderFulfillmentDTO? OrderFulfillment { get; set; }

    public int OrderItemId { get; set; }

    public OrderItemDTO? OrderItem { get; set; }

    public decimal ExpectedQuantity { get; set; }

    public decimal? ReceivedQuantity { get; set; }
}
