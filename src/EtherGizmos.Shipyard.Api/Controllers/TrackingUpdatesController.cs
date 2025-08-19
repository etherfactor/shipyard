using Asp.Versioning;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Api.Controllers;

public class TrackingUpdatesController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/trackingUpdates";

    public TrackingUpdatesController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(TrackingUpdateDTO)), SwaggerResponseExample(200, typeof(TrackingUpdateDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<TrackingUpdateDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [ProducesResponseType(200, Type = typeof(TrackingUpdateDTO)), SwaggerResponseExample(200, typeof(TrackingUpdateDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<TrackingUpdateDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    private IKeylessRequestBuilder<TrackingUpdate, TrackingUpdateDTO> ForSet()
        => ForSet<TrackingUpdate, TrackingUpdateDTO>();

    private IKeyedRequestBuilder<TrackingUpdate, TrackingUpdateDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<TrackingUpdate, TrackingUpdateDTO, int>.Create(id, e => e.Id, e => e.Id));
}
