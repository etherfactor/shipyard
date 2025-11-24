using Asp.Versioning;
using AutoMapper;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Api.Services.Security;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Swashbuckle.AspNetCore.Filters;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Api.Controllers;

[Authorize]
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

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute + "({id})" + "/roles/$ref")]
    [HasCapability(SecurableType.User, PermissionId.Write)]
    [ProducesResponseType(204)]
    public Task<IActionResult> CreateRefToRole(
        Guid id,
        [FromBody] Uri link,
        CancellationToken cancellationToken = default)
        => ForRoleRef(id, ParseRelatedKey<RoleDTO, int>(link, ErrorConstants.RequestTarget.Body))
            .CreateAsync(cancellationToken);

    [ApiVersion(1.0)]
    [HttpDelete(BaseRoute + "({id})" + "/roles/$ref")]
    [HasCapability(SecurableType.User, PermissionId.Write)]
    [ProducesResponseType(204)]
    public Task<IActionResult> DeleteRefToRole(
        Guid id,
        [FromQuery(Name = "$id")] Uri link,
        CancellationToken cancellationToken = default)
        => ForRoleRef(id, ParseRelatedKey<RoleDTO, int>(link, ErrorConstants.RequestTarget.Body))
            .DeleteAsync(cancellationToken);

    private IKeylessRequestBuilder<User, UserDTO> ForSet()
        => ForSet<User, UserDTO>()
            .OnCreating(async (db, dto) =>
            {
                using var uow = _uowFactory.AsUnfiltered().Create();
                var userRepo = uow.Repository<User>();

                Guid.TryParse(User.GetClaim(Claims.Subject), out var userId);
                var groupId = await userRepo.Data
                    .Where(e => e.Id == userId)
                    .Select(e => e.GroupId)
                    .SingleAsync();

                db.GroupId = groupId;
            });

    private IKeyedRequestBuilder<User, UserDTO> ForItem(
        Guid id)
        => ForItem(
            KeyMapping<User, UserDTO, Guid>.Create(id, e => e.Id, e => e.Id));

    private IReferenceRequestBuilder<Role, RoleDTO> ForRoleRef(
        Guid id,
        int roleId)
        => ForItem(id).ForReference(
            e => e.Roles,
            KeyMapping<Role, RoleDTO, int>.Create(roleId, e => e.Id, e => e.Id));
}
