using OpenIddict.Abstractions;
using System.Security.Claims;

namespace EtherGizmos.Common.Abstractions;

public record UpstreamPrincipal(
    OAuth2GrantKind Kind,
    ClaimsPrincipal Value)
{
    public static UpstreamPrincipal Create(
        ClaimsPrincipal principal)
    {
        var kind = OAuth2GrantKindConverter.FromStringOrDefault(principal.GetClaim("gty"))
            ?? OAuth2GrantKind.Unknown;

        return new(kind, principal);
    }
}
