using OpenIddict.Abstractions;
using System.Security.Claims;

namespace EtherGizmos.Common.Extensions;

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
