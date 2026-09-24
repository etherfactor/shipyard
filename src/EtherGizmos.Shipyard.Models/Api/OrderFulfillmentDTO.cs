using EtherGizmos.Shipyard.Api.Enums;

namespace EtherGizmos.Shipyard.Api;

public class OrderFulfillmentDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int OrderId { get; set; }

    public OrderDTO? Order { get; set; }

    public string? Sender { get; set; }

    public OrderFulfillmentStatusTypeDTO OrderFulfillmentStatusType { get; set; }

    public DateOnly? ShippedOn { get; set; }

    public DateTimeOffset? ReceivedAt { get; set; }

    public Guid? ReceivedByUserId { get; set; }

    public UserDTO? ReceivedByUser { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }

    public Guid? ResolvedByUserId { get; set; }

    public UserDTO? ResolvedByUser { get; set; }

    public string? ResolvedReason { get; set; }

    public List<OrderFulfillmentItemDTO> OrderFulfillmentItems { get; set; } = [];
}
