using Asp.Versioning;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Controllers;

public class NotificationMetaController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/";

    public NotificationMetaController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationChannels")]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(NotificationChannelDTO)), SwaggerResponseExample(200, typeof(NotificationChannelDTOExampleGet))]
    public Task<IActionResult> Search(
        ODataQueryOptions<NotificationChannelDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForChannelSet()
            .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationChannels" + "({id})")]
    [ProducesResponseType(200, Type = typeof(NotificationChannelDTO)), SwaggerResponseExample(200, typeof(NotificationChannelDTOExampleGet))]
    public Task<IActionResult> Get(
        string id,
        ODataQueryOptions<NotificationChannelDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForChannelItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationSchedules")]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(NotificationScheduleDTO)), SwaggerResponseExample(200, typeof(NotificationScheduleDTOExampleGet))]
    public Task<IActionResult> Search(
        ODataQueryOptions<NotificationScheduleDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForScheduleSet()
            .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationSchedules" + "({id})")]
    [ProducesResponseType(200, Type = typeof(NotificationScheduleDTO)), SwaggerResponseExample(200, typeof(NotificationScheduleDTOExampleGet))]
    public Task<IActionResult> Get(
        string id,
        ODataQueryOptions<NotificationScheduleDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForScheduleItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationEvents")]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(NotificationEventDTO)), SwaggerResponseExample(200, typeof(NotificationEventDTOExampleGet))]
    public Task<IActionResult> Search(
        ODataQueryOptions<NotificationEventDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForEventSet()
            .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "notificationEvents" + "({id})")]
    [ProducesResponseType(200, Type = typeof(NotificationEventDTO)), SwaggerResponseExample(200, typeof(NotificationEventDTOExampleGet))]
    public Task<IActionResult> Get(
        string id,
        ODataQueryOptions<NotificationEventDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForEventItem(id)
            .GetAsync(queryOptions, cancellationToken);

    private IKeylessRequestBuilder<AppNotificationChannel, NotificationChannelDTO> ForChannelSet()
        => ForSet<AppNotificationChannel, NotificationChannelDTO>();

    private IKeyedRequestBuilder<AppNotificationChannel, NotificationChannelDTO> ForChannelItem(
        string id)
        => ForItem(
            KeyMapping<AppNotificationChannel, NotificationChannelDTO, string>.Create(id, e => e.Id, e => e.Id));

    private IKeylessRequestBuilder<AppNotificationSchedule, NotificationScheduleDTO> ForScheduleSet()
        => ForSet<AppNotificationSchedule, NotificationScheduleDTO>();

    private IKeyedRequestBuilder<AppNotificationSchedule, NotificationScheduleDTO> ForScheduleItem(
        string id)
        => ForItem(
            KeyMapping<AppNotificationSchedule, NotificationScheduleDTO, string>.Create(id, e => e.Id, e => e.Id));

    private IKeylessRequestBuilder<AppNotificationEvent, NotificationEventDTO> ForEventSet()
        => ForSet<AppNotificationEvent, NotificationEventDTO>();

    private IKeyedRequestBuilder<AppNotificationEvent, NotificationEventDTO> ForEventItem(
        string id)
        => ForItem(
            KeyMapping<AppNotificationEvent, NotificationEventDTO, string>.Create(id, e => e.Id, e => e.Id));
}
