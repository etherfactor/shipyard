namespace EtherGizmos.Shipyard.Services.Carriers;

public record TrackingResultDetail
{
    public required DateTimeOffset OccurredAt { get; init; }

    public required int StatusTypeId { get; init; }

    public required string? Location { get; init; }

    public required string? Description { get; init; }
}
