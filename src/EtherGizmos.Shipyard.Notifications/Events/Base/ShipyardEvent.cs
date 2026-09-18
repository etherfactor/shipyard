using EtherGizmos.Common.Abstractions;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public record ShipyardEvent : IDomainEvent
{
    public required string Title { get; init; }

    public required string Message { get; init; }

    public required string ShipyardUrl { get; init; }
}
