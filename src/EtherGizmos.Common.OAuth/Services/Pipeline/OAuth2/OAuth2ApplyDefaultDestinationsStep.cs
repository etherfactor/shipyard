using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Services.Pipeline.OAuth2;

internal class OAuth2ApplyDefaultDestinationsStep : IClaimsPipelineStep<OAuth2PrincipalContext>
{
    public int Order => 900;

    public Task<OAuth2PrincipalContext> ExecuteAsync(
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default)
    {
        context.Identity.SetDestinations(GetDestinations);

        return Task.FromResult(context);
    }

    private IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.ClientId:
            case Claims.Email:
            case Claims.FamilyName:
            case Claims.GivenName:
            case Claims.Name:
            case Claims.Subject:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;
        }

        yield return Destinations.AccessToken;
    }
}
