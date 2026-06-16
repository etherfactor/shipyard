using Asp.Versioning;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Controllers;

[Authorize]
public class NotificationsController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/notifications";

    public NotificationsController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(NotificationDTO)), SwaggerResponseExample(200, typeof(NotificationDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<NotificationDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [ProducesResponseType(200, Type = typeof(NotificationDTO)), SwaggerResponseExample(200, typeof(NotificationDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<NotificationDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    private IKeylessRequestBuilder<AppNotification, NotificationDTO> ForSet()
        => ForSet<AppNotification, NotificationDTO>();

    private IKeyedRequestBuilder<AppNotification, NotificationDTO> ForItem(
        long id)
        => ForItem(
            KeyMapping<AppNotification, NotificationDTO, long>.Create(id, e => e.Id, e => e.Id));
}
