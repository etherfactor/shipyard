using Asp.Versioning;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Converters;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Controllers;

public class NotificationMetaController : ODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/";

    private static readonly JsonSerializerOptions _jsonOptions;
    private readonly INotificationCatalogProvider _catalogProvider;

    public NotificationMetaController(
        INotificationCatalogProvider catalogProvider)
    {
        _catalogProvider = catalogProvider;
    }

    static NotificationMetaController()
    {
        _jsonOptions = new(JsonSerializerOptions.Web)
        {
            Converters =
            {
                new ObjectToInferredTypesConverter(),
            },
        };
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationChannels")]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(NotificationChannelDTO)), SwaggerResponseExample(200, typeof(NotificationChannelDTOExampleGet))]
    public IActionResult Search(
    ODataQueryOptions<NotificationChannelDTO> queryOptions,
    CancellationToken cancellationToken = default)
    {
        var channels = _catalogProvider.GetCatalog().Channels
            .Select(e => new NotificationChannelDTO()
            {
                Id = e.ChannelKey,
                Name = e.DisplayName,
                ConfigSchema = new()
                {
                    Data = e.ConfigSchema.Deserialize<IDictionary<string, object?>>(_jsonOptions)!
                },
            })
            .ToList()
            .AsQueryable();

        var result = queryOptions.ApplyTo(channels);
        return Ok(result);
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationChannels" + "({id})")]
    [ProducesResponseType(200, Type = typeof(NotificationChannelDTO)), SwaggerResponseExample(200, typeof(NotificationChannelDTOExampleGet))]
    public IActionResult Get(
        string id,
        ODataQueryOptions<NotificationChannelDTO> queryOptions,
        CancellationToken cancellationToken = default)
    {
        var channel = _catalogProvider.GetCatalog().Channels
            .Select(MapToDto)
            .FirstOrDefault(e => e.Id == id);

        if (channel is null)
        {
            new Error.Reference.EntityNotFoundReferenceError<NotificationChannelDTO>()
                .AddDetail((e => e.Id, id))
                .Return();
        }

        var result = queryOptions.ApplyTo(channel, new ODataQuerySettings());
        return Ok(result);
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationSchedules")]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(NotificationScheduleDTO)), SwaggerResponseExample(200, typeof(NotificationScheduleDTOExampleGet))]
    public IActionResult Search(
    ODataQueryOptions<NotificationScheduleDTO> queryOptions,
    CancellationToken cancellationToken = default)
    {
        var schedules = _catalogProvider.GetCatalog().Schedules
            .Select(e => new NotificationScheduleDTO()
            {
                Id = e.ScheduleKey,
                Name = e.DisplayName,
                ConfigSchema = new()
                {
                    Data = e.ConfigSchema.Deserialize<IDictionary<string, object?>>(_jsonOptions)!
                },
            })
            .ToList()
            .AsQueryable();

        var result = queryOptions.ApplyTo(schedules);
        return Ok(result);
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationSchedules" + "({id})")]
    [ProducesResponseType(200, Type = typeof(NotificationScheduleDTO)), SwaggerResponseExample(200, typeof(NotificationScheduleDTOExampleGet))]
    public IActionResult Get(
        string id,
        ODataQueryOptions<NotificationScheduleDTO> queryOptions,
        CancellationToken cancellationToken = default)
    {
        var schedule = _catalogProvider.GetCatalog().Schedules
            .Select(MapToDto)
            .FirstOrDefault(e => e.Id == id);

        if (schedule is null)
        {
            new Error.Reference.EntityNotFoundReferenceError<NotificationScheduleDTO>()
                .AddDetail((e => e.Id, id))
                .Return();
        }

        var result = queryOptions.ApplyTo(schedule, new ODataQuerySettings());
        return Ok(result);
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationEvents")]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(NotificationEventDTO)), SwaggerResponseExample(200, typeof(NotificationEventDTOExampleGet))]
    public IActionResult Search(
    ODataQueryOptions<NotificationEventDTO> queryOptions,
    CancellationToken cancellationToken = default)
    {
        var catalog = _catalogProvider.GetCatalog();
        var events = catalog.Events
            .Select(e => new NotificationEventDTO()
            {
                Id = e.EventKey,
                Name = e.DisplayName,
                Supports = [.. e.Supports.Select(s => new NotificationChannelScheduleDTO()
                {
                    NotificationChannelId = s.ChannelKey,
                    NotificationChannel = MapToDto(catalog.Channels.Single(e => e.ChannelKey == s.ChannelKey)),
                    NotificationScheduleId = s.ScheduleKey,
                    NotificationSchedule = MapToDto(catalog.Schedules.Single(e => e.ScheduleKey == s.ScheduleKey)),
                })],
            })
            .ToList()
            .AsQueryable();

        var result = queryOptions.ApplyTo(events);
        return Ok(result);
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationEvents" + "({id})")]
    [ProducesResponseType(200, Type = typeof(NotificationEventDTO)), SwaggerResponseExample(200, typeof(NotificationEventDTOExampleGet))]
    public IActionResult Get(
        string id,
        ODataQueryOptions<NotificationEventDTO> queryOptions,
        CancellationToken cancellationToken = default)
    {
        var @event = _catalogProvider.GetCatalog().Events
            .Select(MapToDto)
            .FirstOrDefault(e => e.Id == id);

        if (@event is null)
        {
            new Error.Reference.EntityNotFoundReferenceError<NotificationEventDTO>()
                .AddDetail((e => e.Id, id))
                .Return();
        }

        var result = queryOptions.ApplyTo(@event, new ODataQuerySettings());
        return Ok(result);
    }

    private static NotificationChannelDTO MapToDto(
        NotificationCatalogChannel channel)
        => new NotificationChannelDTO()
        {
            Id = channel.ChannelKey,
            Name = channel.DisplayName,
            ConfigSchema = new()
            {
                Data = channel.ConfigSchema.Deserialize<IDictionary<string, object?>>(_jsonOptions)!
            },
        };

    private static NotificationScheduleDTO MapToDto(
        NotificationCatalogSchedule schedule)
        => new NotificationScheduleDTO()
        {
            Id = schedule.ScheduleKey,
            Name = schedule.DisplayName,
            ConfigSchema = new()
            {
                Data = schedule.ConfigSchema.Deserialize<IDictionary<string, object?>>(_jsonOptions)!
            },
        };

    private NotificationEventDTO MapToDto(
        NotificationCatalogEvent @event)
        => new NotificationEventDTO()
        {
            Id = @event.EventKey,
            Name = @event.DisplayName,
            Supports = [.. @event.Supports.Select(s => new NotificationChannelScheduleDTO()
            {
                NotificationChannelId = s.ChannelKey,
                NotificationScheduleId = s.ScheduleKey,
            })],
        };
}
