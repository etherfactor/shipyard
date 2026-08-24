using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

internal class PackageEtaChangedExtractor : IDomainEventExtractor
{
    private readonly IOptionsMonitor<WebUIOptions> _selfOptions;

    public bool CanHandle(EntityEntry entry)
        => entry.Metadata.ClrType == typeof(Package);

    public PackageEtaChangedExtractor(
        IOptionsMonitor<WebUIOptions> selfOptions)
    {
        _selfOptions = selfOptions;
    }

    public async IAsyncEnumerable<DomainEventEmission> ExtractAsync(
        EntityEntry entry,
        IUnitOfWork unitOfWork,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var property = entry.Property(nameof(Package.EstimatedDeliveryAt));
        var current = property.CurrentValue as DateTimeOffset?;
        var original = property.OriginalValue as DateTimeOffset?;
        if (current != original && (
            //ETA was cleared out
            current is null
            //A new ETA was added
            || original is null
            //ETA changed by over 12 hours
            || Math.Abs((current.Value - original.Value).TotalHours) >= 12
            //New ETA is within 24 hours of now
            || (current.Value - DateTimeOffset.UtcNow).TotalHours <= 24
            //Previous ETA was within 24 hours of now
            || (original.Value - DateTimeOffset.UtcNow).TotalHours <= 24))
        {
            var uri = new Uri(_selfOptions.CurrentValue.BaseUrl);
            var package = (Package)entry.Entity;

            var lastUpdate = package.TrackingUpdates.OrderBy(e => e.OccurredAt).LastOrDefault();
            var message = $"{package.Carrier.Name} package containing {package.Contents} had its delivery ETA updated from {original} to {current}."
                .Replace("..", ".");

            var @event = new PackageEtaChangedEvent()
            {
                Title = "Package Delivered",
                Message = message,
                ShipyardUrl = $"{uri.Scheme}://{uri.Authority}",

                PackageId = package.Id,
                CarrierId = package.CarrierId,
                CarrierName = package.Carrier.Name,
                TrackingNumber = package.TrackingNumber,
                TrackingUrl = TryGetTrackingUrl(package),
                Contents = package.Contents,
                CurrentEta = current,
                PreviousEta = original,
                Updates = [.. package.TrackingUpdates.Select(e => new PackageEtaChangedEventUpdate()
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

    private string? TryGetTrackingUrl(
        Package package)
    {
        foreach (var step in package.Carrier.Steps)
        {
            if (step.StepType != StepType.Navigate)
                continue;

            step.Payload.TryGetValue("url", out var test);
            if (test is string url)
                return url.Replace("{trackingNumber}", package.TrackingNumber);
        }

        return null;
    }
}
