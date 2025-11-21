using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;

namespace EtherGizmos.Common.Services.Pipeline.OAuth2;

internal class OAuth2SetResourcesStep : IClaimsPipelineStep<OAuth2PrincipalContext>
{
    public int Order => -980;

    public Task<OAuth2PrincipalContext> ExecuteAsync(
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default)
    {
        context.Identity.SetResources(context.Request.Resources);

        return Task.FromResult(context);
    }
}
