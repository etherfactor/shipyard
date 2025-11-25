using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;

namespace EtherGizmos.Common.Services.Pipeline.OAuth2;

internal class OAuth2SetProtocolMetadataStep : IClaimsPipelineStep<OAuth2PrincipalContext>
{
    public int Order => -400;

    public Task<OAuth2PrincipalContext> ExecuteAsync(
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default)
    {
        context.Identity.SetClaim("gty", OAuth2GrantKindConverter.ToString(context.Request.CurrentGrant));
        context.Identity.SetClaim("gty_init", OAuth2GrantKindConverter.ToString(context.Request.OriginalGrant));

        return Task.FromResult(context);
    }
}
