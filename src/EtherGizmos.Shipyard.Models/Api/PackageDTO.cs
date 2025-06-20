using EtherGizmos.Shipyard.Models.Api.Enums;

namespace EtherGizmos.Shipyard.Models.Api;

public class PackageDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int CarrierId { get; set; }

    public CarrierDTO Carrier { get; set; } = null!;

    public string TrackingNumber { get; set; } = null!;

    public string? Contents { get; set; }

    public DateTimeOffset LastPollAt { get; set; }

    public DateTimeOffset NextPollAt { get; set; }

    public StatusTypeDTO StatusType { get; set; }

    public bool IsDelivered { get; set; }
}
