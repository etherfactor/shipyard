using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

internal class PackageReturnedExtractor : IDomainEventExtractor
{
    private readonly IOptionsMonitor<WebUIOptions> _selfOptions;

    public bool CanHandle(EntityEntry entry)
        => entry.Metadata.ClrType == typeof(Package);

    public PackageReturnedExtractor(
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
        if (current == StatusTypeId.Returned
            && current != original)
        {
            var uri = new Uri(_selfOptions.CurrentValue.BaseUrl);
            var package = (Package)entry.Entity;

            var lastUpdate = package.TrackingUpdates.OrderBy(e => e.OccurredAt).LastOrDefault();
            var message = $"{package.Carrier.Name} package containing {package.Contents} is being returned with status: {lastUpdate?.Description}."
                .Replace("..", ".");

            var @event = new PackageReturnedEvent()
            {
                Title = "Package Returned",
                Message = message,
                ShipyardUrl = $"{uri.Scheme}://{uri.Authority}",
                UnsubscribeKey = "invalid",

                PackageId = package.Id,
                CarrierId = package.CarrierId,
                CarrierName = package.Carrier.Name,
                TrackingNumber = package.TrackingNumber,
                TrackingUrl = TryGetTrackingUrl(package),
                Contents = package.Contents,
                Updates = [.. package.TrackingUpdates.Select(e => new PackageReturnedEventUpdate()
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
