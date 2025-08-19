using OpenIddict.Abstractions;
using System.Security.Claims;

namespace EtherGizmos.Shipyard.Api.Extensions;

public static class ClaimsIdentityExtensions
{
    public static void TryAddClaim(
        this ClaimsIdentity @this,
        string claim,
        string? value)
    {
        if (value is not null)
        {
            @this.AddClaim(claim, value);
        }
    }
}
