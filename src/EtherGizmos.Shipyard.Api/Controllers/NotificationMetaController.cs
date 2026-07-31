using Asp.Versioning;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Controllers;

[Authorize]
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

    private IKeylessRequestBuilder<NotificationChannel, NotificationChannelDTO> ForChannelSet()
        => ForSet<NotificationChannel, NotificationChannelDTO>();

    private IKeyedRequestBuilder<NotificationChannel, NotificationChannelDTO> ForChannelItem(
        string id)
        => ForItem(
            KeyMapping<NotificationChannel, NotificationChannelDTO, string>.Create(id, e => e.Id, e => e.Id));

    private IKeylessRequestBuilder<NotificationSchedule, NotificationScheduleDTO> ForScheduleSet()
        => ForSet<NotificationSchedule, NotificationScheduleDTO>();

    private IKeyedRequestBuilder<NotificationSchedule, NotificationScheduleDTO> ForScheduleItem(
        string id)
        => ForItem(
            KeyMapping<NotificationSchedule, NotificationScheduleDTO, string>.Create(id, e => e.Id, e => e.Id));

    private IKeylessRequestBuilder<NotificationEvent, NotificationEventDTO> ForEventSet()
        => ForSet<NotificationEvent, NotificationEventDTO>();

    private IKeyedRequestBuilder<NotificationEvent, NotificationEventDTO> ForEventItem(
        string id)
        => ForItem(
            KeyMapping<NotificationEvent, NotificationEventDTO, string>.Create(id, e => e.Id, e => e.Id));
}
