using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;

namespace EtherGizmos.Common.Services.Pipeline.OAuth2;

internal class OAuth2SetScopesStep : IClaimsPipelineStep<OAuth2PrincipalContext>
{
    public int Order => -990;

    public Task<OAuth2PrincipalContext> ExecuteAsync(
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default)
    {
        context.Identity.SetScopes(context.Request.Scopes);

        return Task.FromResult(context);
    }
}
