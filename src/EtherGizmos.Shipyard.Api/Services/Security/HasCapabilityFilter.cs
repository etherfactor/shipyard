using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenIddict.Abstractions;

namespace EtherGizmos.Shipyard.Api.Services.Security;

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
        var httpUser = context.HttpContext.User;
        if (httpUser is not null)
        {
            var capabilities = httpUser.GetClaim("cap");
            if (capabilities is not null)
            {
                var section = capabilities.Split(';')
                    .SingleOrDefault(e => e.StartsWith($"{_securableType}:"));

                if (section is not null)
                {
                    var value = int.Parse(section.Split(':')[1]);

                    //The user has the permission. Bitwise AND checks that both numbers share at least one bit
                    if ((_permissionId & value) != 0)
                    {
                        return;
                    }
                }
            }
        }

        //The user does not exist or does not have the capability
        new Error.Authorization.MissingPermissionError()
            .AddDetail(_securableType, _permissionId)
            .Return();
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
