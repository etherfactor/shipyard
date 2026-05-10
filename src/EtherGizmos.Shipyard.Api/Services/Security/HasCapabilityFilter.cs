using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EtherGizmos.Shipyard.Services.Security;

public class HasCapabilityFilter : IAuthorizationFilter
{
    private readonly SecurableType _securableType;
    private readonly int _permissionId;

    public HasCapabilityFilter(
        SecurableType securableType,
        int permissionId)
    {
        _securableType = securableType;
        _permissionId = permissionId;
    }

    public void OnAuthorization(
        AuthorizationFilterContext context)
    {
        var authorizer = context.HttpContext.RequestServices.GetRequiredService<ICapabilityAuthorizer>();
        authorizer.EnsureAuthorized(_securableType, _permissionId);
    }
}

public class HasCapabilityAttribute : TypeFilterAttribute
{
    public SecurableType SecurableType { get; }

    public int PermissionId { get; }

    public HasCapabilityAttribute(
        SecurableType securableType,
        int permissionId)
        : base(typeof(HasCapabilityFilter))
    {
        SecurableType = securableType;
        PermissionId = permissionId;

        Arguments = [SecurableType, PermissionId];
    }
}
