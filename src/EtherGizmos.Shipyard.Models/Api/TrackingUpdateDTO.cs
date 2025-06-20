using EtherGizmos.Shipyard.Models.Api.Enums;

namespace EtherGizmos.Shipyard.Models.Api;

public class TrackingUpdateDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int PackageId { get; set; }

    public PackageDTO Package { get; set; } = null!;

    public DateTimeOffset OccurredAt { get; set; }

    public StatusTypeDTO StatusType { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }
}
