using Asp.Versioning;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Api.Errors;
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

    private readonly INotificationUnsubscribeService _unsubscribeService;
    private readonly IUserContext _userContext;

    public NotificationSubscriptionsController(
        IServiceProvider serviceProvider,
        INotificationUnsubscribeService unsubscribeService,
        IUserContext userContext)
        : base(serviceProvider)
    {
        _unsubscribeService = unsubscribeService;
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

    [AllowAnonymous]
    [ApiVersion(1.0)]
    [HttpPost(BaseRoute + "({id})/unsubscribe")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Unsubscribe(
        int id,
        string key,
        CancellationToken cancellationToken = default)
    {
        var result = await _unsubscribeService.UnsubscribeAsync(id, key, cancellationToken);
        if (!result)
        {
            new Error.Reference.EntityNotFoundReferenceError<NotificationSubscriptionDTO>()
                .AddDetail((e => e.Id, id))
                .Return();
        }

        return NoContent();
    }

    private IKeylessRequestBuilder<NotificationSubscription, NotificationSubscriptionDTO> ForSet()
        => ForSet<NotificationSubscription, NotificationSubscriptionDTO>()
            .OnCreating(async (db, dto) =>
            {
                db.UserId = _userContext.UserId.ToString()!;
            });

    private IKeyedRequestBuilder<NotificationSubscription, NotificationSubscriptionDTO> ForItem(
        long id)
        => ForItem(
            KeyMapping<NotificationSubscription, NotificationSubscriptionDTO, long>.Create(id, e => e.Id, e => e.Id));
}
