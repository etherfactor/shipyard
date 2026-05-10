using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Database.Enums;
using OpenIddict.Abstractions;

namespace EtherGizmos.Shipyard.Services.Security;

internal class CapabilityAuthorizer : ICapabilityAuthorizer
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CapabilityAuthorizer(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void EnsureAuthorized(SecurableType securableType, int permissionId)
    {
        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException();

        var httpUser = context.User;
        if (httpUser is not null)
        {
            var capabilities = httpUser.GetClaim("cap");
            if (capabilities is not null)
            {
                var section = capabilities.Split(';')
                    .SingleOrDefault(e => e.StartsWith($"{securableType}:"));

                if (section is not null)
                {
                    var value = int.Parse(section.Split(':')[1]);

                    //The user has the permission. Bitwise AND checks that both numbers share at least one bit
                    if ((permissionId & value) != 0)
                    {
                        return;
                    }
                }
            }
        }

        //The user does not exist or does not have the capability
        new Error.Authorization.MissingPermissionError()
            .AddDetail(securableType, permissionId)
            .Return();
    }
}
