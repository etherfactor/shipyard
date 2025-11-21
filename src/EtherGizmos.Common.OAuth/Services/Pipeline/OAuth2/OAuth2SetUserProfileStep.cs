using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Services.Pipeline.OAuth2;

internal class OAuth2SetUserProfileStep<TUser> : IClaimsPipelineStep<OAuth2PrincipalContext>
    where TUser : class, IUser
{
    public int Order => 0;

    private readonly IUserStore<TUser> _userStore;

    public OAuth2SetUserProfileStep(
        IUserStore<TUser> userStore)
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
            context.Identity.SetClaim(Claims.GivenName, user.GivenName);
            context.Identity.SetClaim(Claims.FamilyName, user.FamilyName);
            context.Identity.SetClaim(Claims.Name, user.FullName);

            context.Identity.SetClaim(Claims.Email, user.EmailAddress);
        }

        return context;
    }
}
