using EtherGizmos.Common.Abstractions;
using System.Security.Claims;

namespace EtherGizmos.Common.Services;

internal class CookiePrincipalFactory<TUser> : ICookiePrincipalFactory<TUser>
    where TUser : class, IUser
{
    private readonly IEnumerable<IClaimsPipelineStep<CookiePrincipalContext<TUser>>> _steps;

    public CookiePrincipalFactory(
        IEnumerable<IClaimsPipelineStep<CookiePrincipalContext<TUser>>> steps)
    {
        _steps = steps;
    }

    public async Task<ClaimsPrincipal> CreateAsync(
        string authenticationType,
        CookiePrincipalContext<TUser> context,
        CancellationToken cancellationToken = default)
    {
        foreach (var step in _steps)
        {
            context = await step.ExecuteAsync(context, cancellationToken);
        }

        var identity = new ClaimsIdentity(context.Claims, authenticationType);
        return new(identity);
    }
}
