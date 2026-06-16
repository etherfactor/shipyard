using Asp.Versioning;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Controllers;

[Authorize]
public class NotificationSubscriptionsController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/notificationSubscriptions";

    private readonly INotificationCatalogProvider _catalogProvider;
    private readonly IUserContext _userContext;

    public NotificationSubscriptionsController(
        IServiceProvider serviceProvider,
        INotificationCatalogProvider catalogProvider,
        IUserContext userContext)
        : base(serviceProvider)
    {
        _catalogProvider = catalogProvider;
        _userContext = userContext;
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    //[HasCapability(SecurableType.NotificationSubscription, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(NotificationSubscriptionDTO)), SwaggerResponseExample(200, typeof(NotificationSubscriptionDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<NotificationSubscriptionDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    //[HasCapability(SecurableType.NotificationSubscription, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(NotificationSubscriptionDTO)), SwaggerResponseExample(200, typeof(NotificationSubscriptionDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<NotificationSubscriptionDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    //[HasCapability(SecurableType.NotificationSubscription, PermissionId.Write)]
    [Consumes(typeof(NotificationSubscriptionDTO), "application/json"), SwaggerRequestExample(typeof(NotificationSubscriptionDTO), typeof(NotificationSubscriptionDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(NotificationSubscriptionDTO)), SwaggerResponseExample(200, typeof(NotificationSubscriptionDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] NotificationSubscriptionDTO create,
        ODataQueryOptions<NotificationSubscriptionDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    //[HasCapability(SecurableType.NotificationSubscription, PermissionId.Write)]
    [Consumes(typeof(NotificationSubscriptionDTO), "application/json"), SwaggerRequestExample(typeof(NotificationSubscriptionDTO), typeof(NotificationSubscriptionDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(NotificationSubscriptionDTO)), SwaggerResponseExample(200, typeof(NotificationSubscriptionDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<NotificationSubscriptionDTO> patch,
        ODataQueryOptions<NotificationSubscriptionDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .PatchAsync(patch, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpDelete(BaseRoute + "({id})")]
    //[HasCapability(SecurableType.NotificationSubscription, PermissionId.Delete)]
    [ProducesResponseType(204)]
    public Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .DeleteAsync(cancellationToken);

    private IKeylessRequestBuilder<AppNotificationSubscription, NotificationSubscriptionDTO> ForSet()
        => ForSet<AppNotificationSubscription, NotificationSubscriptionDTO>()
            .OnCreating(async (db, dto) =>
            {
                db.UserId = _userContext.UserId.ToString()!;
            });

    private IKeyedRequestBuilder<AppNotificationSubscription, NotificationSubscriptionDTO> ForItem(
        long id)
        => ForItem(
            KeyMapping<AppNotificationSubscription, NotificationSubscriptionDTO, long>.Create(id, e => e.Id, e => e.Id));
}
