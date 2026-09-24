using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Database;

public class TrackingUpdate : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual int PackageId { get; set; }

    public virtual Package Package { get; set; } = null!;

    public virtual DateTimeOffset OccurredAt { get; set; }

    public virtual int TrackingStatusTypeId { get; set; }

    public virtual TrackingStatusType TrackingStatusType { get; set; } = null!;

    public virtual string? Location { get; set; }

    public virtual string? Description { get; set; }
}

public class TrackingUpdateComparer : IEqualityComparer<TrackingUpdate>
{
    private static readonly TimeSpan ROUND_INTERVAL = TimeSpan.FromMinutes(5);

    public bool Equals(TrackingUpdate? x, TrackingUpdate? y)
    {
        if (x is null || y is null)
            return x == y;

        return x.OccurredAt.Round(ROUND_INTERVAL) == y.OccurredAt.Round(ROUND_INTERVAL)
            && x.TrackingStatusTypeId == y.TrackingStatusTypeId
            && x.Location == y.Location;
    }

    public int GetHashCode([DisallowNull] TrackingUpdate obj)
    {
        return HashCode.Combine(
            obj.OccurredAt,
            obj.TrackingStatusTypeId,
            obj.Location ?? "");
    }
}
