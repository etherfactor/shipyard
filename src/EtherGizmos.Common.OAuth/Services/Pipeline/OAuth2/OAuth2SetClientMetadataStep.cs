using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Services.Pipeline.OAuth2;

internal class OAuth2SetClientMetadataStep : IClaimsPipelineStep<OAuth2PrincipalContext>
{
    public int Order => -500;

    private readonly IOpenIddictApplicationManager _applicationManager;

    public OAuth2SetClientMetadataStep(
        IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    public async Task<OAuth2PrincipalContext> ExecuteAsync(
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default)
    {
        var clientId = context.Request.OpenIddict.ClientId;
        context.Identity.SetClaim(Claims.ClientId, clientId);

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            var application = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken: cancellationToken);
            if (application is not null)
            {
                var clientName = await _applicationManager.GetDisplayNameAsync(application, cancellationToken: cancellationToken);
                context.Identity.SetClaim("client_name", clientName);
            }
        }

        return context;
    }
}
