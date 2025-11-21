using EtherGizmos.Common.Abstractions;
using System.Security.Claims;

namespace EtherGizmos.Common.Services;

internal class OAuth2PrincipalFactory : IOAuth2PrincipalFactory
{
    private readonly IEnumerable<IClaimsPipelineStep<OAuth2PrincipalContext>> _steps;

    public OAuth2PrincipalFactory(
        IEnumerable<IClaimsPipelineStep<OAuth2PrincipalContext>> steps)
    {
        _steps = [.. steps.OrderBy(e => e.Order)];
    }

    public async Task<ClaimsPrincipal> CreateAsync(
        string authenticationType,
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default)
    {
        foreach (var step in _steps)
        {
            context = await step.ExecuteAsync(context, cancellationToken);
        }

        var identity = new ClaimsIdentity(context.Identity.Claims, authenticationType);
        return new(identity);
    }
}
