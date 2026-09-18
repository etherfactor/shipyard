using Asp.Versioning;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Api;
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

    private readonly IFilterContext _filterContext;
    private readonly IUserContext _userContext;

    public NotificationsController(
        IServiceProvider serviceProvider,
        IFilterContext filterContext,
        IUserContext userContext)
        : base(serviceProvider)
    {
        _filterContext = filterContext;
        _userContext = userContext;
    }

    protected override IQueryable<TEntity> Filter<TEntity>(
        IQueryable<TEntity> queryable)
    {
        if (queryable is IQueryable<Notification> notificationQueryable)
            return (IQueryable<TEntity>)notificationQueryable.Where(e =>
                _filterContext.Disabled
                || (e.Subscription.UserId != null
                && e.Subscription.UserId == _userContext.UserId.ToString()));

        return queryable;
    }

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

    private IKeylessRequestBuilder<Notification, NotificationDTO> ForSet()
        => ForSet<Notification, NotificationDTO>();

    private IKeyedRequestBuilder<Notification, NotificationDTO> ForItem(
        long id)
        => ForItem(
            KeyMapping<Notification, NotificationDTO, long>.Create(id, e => e.Id, e => e.Id));
}
