using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Converters;
using EtherGizmos.Shipyard.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using DatabaseNotificationChannel = EtherGizmos.Shipyard.Database.AppNotificationChannel;
using DatabaseNotificationEvent = EtherGizmos.Shipyard.Database.AppNotificationEvent;
using DatabaseNotificationEventChannelSchedule = EtherGizmos.Shipyard.Database.AppNotificationEventChannelSchedule;
using DatabaseNotificationSchedule = EtherGizmos.Shipyard.Database.AppNotificationSchedule;

namespace EtherGizmos.Shipyard.Services.Bootstrappers;

public class NotificationBootstrapper : IBootstrapper
{
    public int Order => 900;

    private static readonly JsonSerializerOptions _jsonOptions;
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly INotificationCatalogProvider _catalogProvider;

    public NotificationBootstrapper(
        IUnitOfWorkFactory uowFactory,
        INotificationCatalogProvider catalogProvider)
    {
        _uowFactory = uowFactory;
        _catalogProvider = catalogProvider;
    }

    static NotificationBootstrapper()
    {
        _jsonOptions = new(JsonSerializerOptions.Web)
        {
            Converters =
            {
                new ObjectToInferredTypesConverter(),
            },
        };
    }

    public async Task ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();

        var catalog = _catalogProvider.GetCatalog();

        //Sync channels
        var channelRepo = uow.Repository<DatabaseNotificationChannel>();
        var currentChannelsRaw = await channelRepo.Data
            .ToListAsync(cancellationToken: cancellationToken);

        var currentChannels = currentChannelsRaw.Select(
            e => new NotificationChannelMeta(e.Id, e.Name, JsonSerializer.Serialize(e.ConfigSchema, _jsonOptions)));

        var compareChannels = catalog.Channels.Select(
            e => new NotificationChannelMeta(e.Id, e.Name, JsonSerializer.Serialize(e.ConfigSchema, _jsonOptions)))
            .ToList();

        var addChannels = compareChannels.Except(currentChannels);
        var remChannels = currentChannels.Except(compareChannels);

        foreach (var channel in compareChannels)
        {
            var current = currentChannelsRaw.SingleOrDefault(e => e.Id.Equals(channel.Id, StringComparison.OrdinalIgnoreCase));
            current?.Name = channel.Name;
            current?.ConfigSchema = JsonSerializer.Deserialize<IDictionary<string, object?>>(channel.ConfigSchema, _jsonOptions)!;
        }

        foreach (var channel in addChannels)
        {
            channelRepo.Add(new()
            {
                Id = channel.Id,
                Name = channel.Name,
                ConfigSchema = JsonSerializer.Deserialize<IDictionary<string, object?>>(channel.ConfigSchema, _jsonOptions)!,
            });
        }

        foreach (var channel in remChannels)
        {
            var toRemove = currentChannelsRaw.Single(e => e.Id.Equals(channel.Id, StringComparison.OrdinalIgnoreCase));
            channelRepo.Remove(toRemove);
        }

        //Sync schedules
        var scheduleRepo = uow.Repository<DatabaseNotificationSchedule>();
        var currentSchedulesRaw = await scheduleRepo.Data
            .ToListAsync(cancellationToken: cancellationToken);

        var currentSchedules = currentSchedulesRaw.Select(
            e => new NotificationScheduleMeta(e.Id, e.Name, JsonSerializer.Serialize(e.ConfigSchema, _jsonOptions)));

        var compareSchedules = catalog.Schedules.Select(
            e => new NotificationScheduleMeta(e.Id, e.Name, JsonSerializer.Serialize(e.ConfigSchema, _jsonOptions)))
            .ToList();

        var addSchedules = compareSchedules.Except(currentSchedules);
        var remSchedules = currentSchedules.Except(compareSchedules);

        foreach (var schedule in compareSchedules)
        {
            var current = currentSchedulesRaw.SingleOrDefault(e => e.Id.Equals(schedule.Id, StringComparison.OrdinalIgnoreCase));
            current?.Name = schedule.Name;
            current?.ConfigSchema = JsonSerializer.Deserialize<IDictionary<string, object?>>(schedule.ConfigSchema, _jsonOptions)!;
        }

        foreach (var schedule in addSchedules)
        {
            scheduleRepo.Add(new()
            {
                Id = schedule.Id,
                Name = schedule.Name,
                ConfigSchema = JsonSerializer.Deserialize<IDictionary<string, object?>>(schedule.ConfigSchema, _jsonOptions)!,
            });
        }

        foreach (var schedule in remSchedules)
        {
            var toRemove = currentSchedulesRaw.Single(e => e.Id.Equals(schedule.Id, StringComparison.OrdinalIgnoreCase));
            scheduleRepo.Remove(toRemove);
        }

        //Sync events
        var eventRepo = uow.Repository<DatabaseNotificationEvent>();
        var currentEventsRaw = await eventRepo.Data
            .ToListAsync(cancellationToken: cancellationToken);

        var currentEvents = currentEventsRaw.Select(
            e => new NotificationEventMeta(e.Id, e.Name));

        var compareEvents = catalog.Events.Select(
            e => new NotificationEventMeta(e.Id, e.Name))
            .ToList();

        var addEvents = compareEvents.Except(currentEvents);
        var remEvents = currentEvents.Except(compareEvents);

        foreach (var @event in compareEvents)
        {
            var current = currentEventsRaw.SingleOrDefault(e => e.Id.Equals(@event.Id, StringComparison.OrdinalIgnoreCase));
            current?.Name = @event.Name;
        }

        foreach (var @event in addEvents)
        {
            eventRepo.Add(new()
            {
                Id = @event.Id,
                Name = @event.Name,
            });
        }

        foreach (var @event in remEvents)
        {
            var toRemove = currentEventsRaw.Single(e => e.Id.Equals(@event.Id, StringComparison.OrdinalIgnoreCase));
            eventRepo.Remove(toRemove);
        }

        //Sync event channel schedules
        var eventChannelScheduleRepo = uow.Repository<DatabaseNotificationEventChannelSchedule>();
        var currentEventChannelSchedulesRaw = await eventChannelScheduleRepo.Data
            .ToListAsync(cancellationToken: cancellationToken);

        var currentEventChannelSchedules = currentEventChannelSchedulesRaw.Select(
            e => new NotificationEventChannelScheduleMeta(e.NotificationEventId, e.NotificationChannelId, e.NotificationScheduleId));

        var compareEventChannelSchedules = catalog.Events.Select(
            e => e.Supports.Select(s => new NotificationEventChannelScheduleMeta(e.Id, s.ChannelId, s.ScheduleId)))
            .SelectMany(e => e)
            .ToList();

        var addEventChannelSchedules = compareEventChannelSchedules.Except(currentEventChannelSchedules);
        var remEventChannelSchedules = currentEventChannelSchedules.Except(compareEventChannelSchedules);

        foreach (var eventChannelSchedule in addEventChannelSchedules)
        {
            eventChannelScheduleRepo.Add(new()
            {
                NotificationEventId = eventChannelSchedule.EventId,
                NotificationChannelId = eventChannelSchedule.ChannelId,
                NotificationScheduleId = eventChannelSchedule.ScheduleId,
            });
        }

        foreach (var eventChannelSchedule in remEventChannelSchedules)
        {
            var toRemove = currentEventChannelSchedulesRaw.Single(e =>
                e.NotificationEventId.Equals(eventChannelSchedule.EventId, StringComparison.OrdinalIgnoreCase)
                && e.NotificationChannelId.Equals(eventChannelSchedule.ChannelId, StringComparison.OrdinalIgnoreCase)
                && e.NotificationScheduleId.Equals(eventChannelSchedule.ScheduleId, StringComparison.OrdinalIgnoreCase));
            eventChannelScheduleRepo.Remove(toRemove);
        }

        await uow.SaveChangesAsync(cancellationToken);
    }

    private record NotificationChannelMeta(
        string Id,
        string Name,
        string ConfigSchema);

    private record NotificationScheduleMeta(
        string Id,
        string Name,
        string ConfigSchema);

    private record NotificationEventMeta(
        string Id,
        string Name);

    private record NotificationEventChannelScheduleMeta(
        string EventId,
        string ChannelId,
        string ScheduleId);
}
