namespace EtherGizmos.Shipyard.Database;

public class OrderFulfillmentItem
{
    public virtual int Id { get; set; }

    public virtual int OrderFulfillmentId { get; set; }

    public virtual OrderFulfillment OrderFulfillment { get; set; } = null!;

    public virtual int OrderItemId { get; set; }

    public virtual OrderItem OrderItem { get; set; } = null!;

    public virtual decimal ExpectedQuantity { get; set; }

    public virtual decimal? ReceivedQuantity { get; set; }
}
