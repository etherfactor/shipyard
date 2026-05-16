using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

internal class PackageDeliveredExtractor : IDomainEventExtractor
{
    public bool CanHandle(EntityEntry entry)
        => entry.Metadata.ClrType == typeof(Package);

    public async IAsyncEnumerable<DomainEventEmission> ExtractAsync(
        EntityEntry entry,
        IUnitOfWork unitOfWork,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var property = entry.Property(nameof(Package.LastStatusTypeId));
        var current = property.CurrentValue as int?;
        var original = property.OriginalValue as int?;
        if (current == StatusTypeId.Delivered
            && current != original)
        {
            var package = (Package)entry.CurrentValues.ToObject();
            var @event = new PackageDeliveredEvent()
            {
                ShipyardUrl = "https://shipyard.ethergizmos.com",
                UnsubscribeKey = "invalid",

                PackageId = package.Id,
                CarrierId = package.CarrierId,
                CarrierName = package.Carrier.Name,
                TrackingNumber = package.TrackingNumber,
                Contents = package.Contents,
                Updates = [.. package.TrackingUpdates.Select(e => new PackageDeliveredEventUpdate()
                {
                    Status = e.StatusType.Name,
                    OccurredAt = e.OccurredAt,
                    Location = e.Location,
                    Description = e.Description,
                })],
            };

            var audience = new AudienceKey("package", package.Id.ToString());

            yield return new(@event, [audience]);
        }
    }
}
