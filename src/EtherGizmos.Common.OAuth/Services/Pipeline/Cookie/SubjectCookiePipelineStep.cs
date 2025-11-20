using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Common.Services.Pipeline.Cookie;

internal class SubjectCookiePipelineStep<TUser> : IClaimsPipelineStep<CookiePrincipalContext<TUser>>
    where TUser : class, IUser
{
    public int Order => -999;

    public Task<CookiePrincipalContext<TUser>> ExecuteAsync(
        CookiePrincipalContext<TUser> context,
        CancellationToken cancellationToken = default)
    {
        //return Task.FromResult(
        //    context.WithClaim(new(Claims.Subject, context.));

        throw new NotImplementedException();
    }
}
