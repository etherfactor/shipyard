using EtherGizmos.Common.Messaging.Abstractions;
using EtherGizmos.Shipyard.Database.Services;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Models.Database.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.Shipyard.Worker.Consumers;

public class TrackingResponseConsumer : IMessageConsumer<TrackingResponse>
{
    private static readonly TrackingUpdateComparer TRACKING_COMPARER = new();

    private readonly ILogger _logger;
    private readonly IUnitOfWorkFactory _uowFactory;

    public TrackingResponseConsumer(
        ILogger<TrackingResponseConsumer> logger,
        IUnitOfWorkFactory uowFactory)
    {
        _logger = logger;
        _uowFactory = uowFactory;
    }

    public async Task ConsumeAsync(
        IMessageContext<TrackingResponse> context)
    {
        using var uow = _uowFactory.Create();
        var packageRepo = uow.Repository<Package>();

        var message = context.Message;
        _logger.LogInformation("Received response message {@Message}", message);

        var package = await packageRepo.Data.SingleAsync(e => e.Id == message.PackageId, cancellationToken: context.CancellationToken);

        var newDetails = message.Details
            .Select(e => new TrackingUpdate
            {
                PackageId = package.Id,
                OccurredAt = e.OccurredAt,
                StatusTypeId = e.StatusTypeId,
                Location = e.Location,
                Description = e.Description,
            });

        var existingDetails = package.TrackingUpdates;

        var toAdd = newDetails
            .Except(existingDetails, TRACKING_COMPARER)
            .ToList();

        var toRemove = existingDetails
            .Except(newDetails, TRACKING_COMPARER)
            .ToList();

        foreach (var add in toAdd)
        {
            package.TrackingUpdates.Add(add);
        }

        foreach (var remove in toRemove)
        {
            package.TrackingUpdates.Remove(remove);
        }

        package.EstimatedDeliveryAt = message.EstimatedDeliveryAt;

        package.LastStatusTypeId = package
            .TrackingUpdates
            .OrderBy(e => e.OccurredAt)
            .Select(e => e.StatusTypeId as int?)
            .LastOrDefault() ?? StatusTypeId.Unknown;

        package.IsDelivered = package.LastStatusTypeId == StatusTypeId.Delivered;

        var statusTypeRepo = uow.Repository<StatusType>();
        var statusType = await statusTypeRepo.Data.SingleAsync(e => e.Id == package.LastStatusTypeId, cancellationToken: context.CancellationToken);

        if (statusType.IsFinal)
        {
            package.NextPollAt = DateTimeOffset.MaxValue;
        }
        else
        {
            package.NextPollAt = package.LastPollAt
                + TimeSpan.FromHours(6) * (double)statusType.PollingFactor;
        }

        await uow.SaveChangesAsync(context.CancellationToken);
    }
}

public record TrackingResponse
{
    public int PackageId { get; init; }

    public DateTimeOffset? EstimatedDeliveryAt { get; init; }

    public IReadOnlyList<TrackingResponseDetail> Details { get; init; } = [];
}

public record TrackingResponseDetail
{
    public DateTimeOffset OccurredAt { get; init; }

    public int StatusTypeId { get; init; }

    public string? Location { get; init; }

    public string? Description { get; init; }
}
