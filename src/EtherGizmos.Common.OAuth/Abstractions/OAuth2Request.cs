using OpenIddict.Abstractions;

namespace EtherGizmos.Common.Abstractions;

public record OAuth2Request(
    OpenIddictRequest OpenIddict,
    OAuth2GrantKind CurrentGrant,
    OAuth2GrantKind OriginalGrant)
{
    public IEnumerable<string> Scopes => OpenIddict.GetScopes();

    public IEnumerable<string> Resources => OpenIddict.GetResources();

    public static OAuth2Request Create(
        OpenIddictRequest openIddict,
        OAuth2GrantKind currentGrant,
        OAuth2GrantKind? originalGrant = null)
        => new(openIddict, currentGrant, originalGrant ?? currentGrant);
}
