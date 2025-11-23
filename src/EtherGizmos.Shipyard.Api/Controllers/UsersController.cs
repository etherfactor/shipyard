using Asp.Versioning;
using AutoMapper;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Services.Security;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Api.Controllers;

public class UsersController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/users";

    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IMapper _mapper;

    public UsersController(
        IServiceProvider serviceProvider,
        IUnitOfWorkFactory uowFactory,
        IMapper mapper)
        : base(serviceProvider)
    {
        _uowFactory = uowFactory;
        _mapper = mapper;
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.User, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(UserDTO)), SwaggerResponseExample(200, typeof(UserDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<UserDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.User, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(UserDTO)), SwaggerResponseExample(200, typeof(UserDTOExampleGet))]
    public Task<IActionResult> Get(
        Guid id,
        ODataQueryOptions<UserDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [HasCapability(SecurableType.User, PermissionId.Write)]
    [Consumes(typeof(UserDTO), "application/json"), SwaggerRequestExample(typeof(UserDTO), typeof(UserDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(UserDTO)), SwaggerResponseExample(200, typeof(UserDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] UserDTO create,
        ODataQueryOptions<UserDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.User, PermissionId.Write)]
    [Consumes(typeof(UserDTO), "application/json"), SwaggerRequestExample(typeof(UserDTO), typeof(UserDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(UserDTO)), SwaggerResponseExample(200, typeof(UserDTOExampleGet))]
    public Task<IActionResult> Patch(
        Guid id,
        [FromBody] Delta<UserDTO> patch,
        ODataQueryOptions<UserDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .PatchAsync(patch, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpDelete(BaseRoute + "({id})")]
    [HasCapability(SecurableType.User, PermissionId.Delete)]
    [ProducesResponseType(204)]
    public Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .DeleteAsync(cancellationToken);

    private IKeylessRequestBuilder<User, UserDTO> ForSet()
        => ForSet<User, UserDTO>();

    private IKeyedRequestBuilder<User, UserDTO> ForItem(
        Guid id)
        => ForItem(
            KeyMapping<User, UserDTO, Guid>.Create(id, e => e.Id, e => e.Id));
}
