using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Services.Pipeline.Cookie;

internal class CookieSetSubjectStep<TUser> : IClaimsPipelineStep<CookiePrincipalContext<TUser>>
    where TUser : class, IUser
{
    public int Order => -999;

    private readonly IUserStore<TUser> _userStore;

    public CookieSetSubjectStep(
        IUserStore<TUser> userStore)
    {
        _userStore = userStore;
    }

    public async Task<CookiePrincipalContext<TUser>> ExecuteAsync(
        CookiePrincipalContext<TUser> context,
        CancellationToken cancellationToken = default)
    {
        var subject = await _userStore.GetSubjectAsync(context.User, cancellationToken);
        context.Identity.SetClaim(Claims.Subject, subject);

        return context;
    }
}
