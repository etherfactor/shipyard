using EtherGizmos.Shipyard.Utilities.Abstractions;

namespace EtherGizmos.Shipyard.Models.Database;

public class TrackingUpdate : Auditable
{
    public virtual int Id { get; set; }

    public virtual int PackageId { get; set; }

    public virtual DateTimeOffset OccurredAt { get; set; }

    public virtual int StatusTypeId { get; set; }

    public virtual StatusType StatusType { get; set; } = null!;

    public virtual string? Location { get; set; }

    public virtual string? Description { get; set; }
}
