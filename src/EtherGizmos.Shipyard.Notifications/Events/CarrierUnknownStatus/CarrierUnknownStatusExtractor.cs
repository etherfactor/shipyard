using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

internal class CarrierUnknownStatusExtractor : IDomainEventExtractor
{
    private readonly IOptionsMonitor<WebUIOptions> _selfOptions;

    public bool CanHandle(EntityEntry entry)
        => entry.Metadata.ClrType == typeof(Package);

    public CarrierUnknownStatusExtractor(
        IOptionsMonitor<WebUIOptions> selfOptions)
    {
        _selfOptions = selfOptions;
    }

    public async IAsyncEnumerable<DomainEventEmission> ExtractAsync(
        EntityEntry entry,
        IUnitOfWork unitOfWork,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var property = entry.Property(nameof(Package.LastStatusTypeId));
        var current = property.CurrentValue as int?;
        var original = property.OriginalValue as int?;
        if (current == StatusTypeId.Unknown
            && current != original)
        {
            var uri = new Uri(_selfOptions.CurrentValue.BaseUrl);
            var package = (Package)entry.Entity;
            var lastUpdate = package.TrackingUpdates.OrderBy(e => e.OccurredAt).LastOrDefault();

            var @event = new CarrierUnknownStatusEvent()
            {
                Title = "Unknown Carrier Status",
                Message = $"{package.Carrier.Name} emitted an unknown tracking status: {lastUpdate?.Description}.".Replace("..", "."),
                ShipyardUrl = $"{uri.Scheme}://{uri.Authority}",
                UnsubscribeKey = "invalid",

                CarrierId = package.CarrierId,
                CarrierName = package.Carrier.Name,
                ObservedAt = lastUpdate?.OccurredAt ?? DateTimeOffset.UtcNow,
                StatusText = lastUpdate?.Description,
            };

            var audience = new AudienceKey("carrier", package.CarrierId.ToString());

            yield return new(@event, [audience]);
        }
    }
}
