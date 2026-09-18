using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Services.Pipeline.OAuth2;

public class OAuth2ApplyDestinationsStep : IClaimsPipelineStep<OAuth2PrincipalContext>
{
    public int Order => 950;

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
            case Claims.Username:
            case "cap":
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;
        }

        yield return Destinations.AccessToken;
    }
}
