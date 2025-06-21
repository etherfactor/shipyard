namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

public record TrackingResultDetail
{
    public required DateTimeOffset EventOccurredAt { get; init; }

    public required int StatusTypeId { get; init; }

    public required string? Location { get; init; }

    public required string? Description { get; init; }
}
