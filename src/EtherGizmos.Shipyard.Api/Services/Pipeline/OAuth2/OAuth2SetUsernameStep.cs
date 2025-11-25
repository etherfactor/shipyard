using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Api.Services.Pipeline.OAuth2;

public class OAuth2SetUsernameStep : IClaimsPipelineStep<OAuth2PrincipalContext>
{
    public int Order => 350;

    private readonly IUserStore<User> _userStore;

    public OAuth2SetUsernameStep(
        IUserStore<User> userStore)
    {
        _userStore = userStore;
    }

    public async Task<OAuth2PrincipalContext> ExecuteAsync(
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default)
    {
        var subject = context.Subject.Value;
        var user = await _userStore.FindBySubjectAsync(subject, cancellationToken: cancellationToken);

        if (user is not null)
        {
            context.Identity.SetClaim(Claims.Username, user.Username);
        }

        return context;
    }
}
