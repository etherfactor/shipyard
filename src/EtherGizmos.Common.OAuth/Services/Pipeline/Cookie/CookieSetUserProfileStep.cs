using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Services.Pipeline.Cookie;

internal class CookieSetUserProfileStep<TUser>
    : IClaimsPipelineStep<CookiePrincipalContext<TUser>>
    where TUser : class, IUser
{
    public int Order => 0;

    public Task<CookiePrincipalContext<TUser>> ExecuteAsync(
        CookiePrincipalContext<TUser> context,
        CancellationToken cancellationToken = default)
    {
        context.Identity.SetClaim(Claims.GivenName, context.User.GivenName);
        context.Identity.SetClaim(Claims.FamilyName, context.User.FamilyName);
        context.Identity.SetClaim(Claims.Name, context.User.FullName);

        context.Identity.SetClaim(Claims.Email, context.User.EmailAddress);

        return Task.FromResult(context);
    }
}
