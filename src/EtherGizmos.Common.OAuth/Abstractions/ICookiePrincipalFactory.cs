using System.Security.Claims;

namespace EtherGizmos.Common.Abstractions;

public interface ICookiePrincipalFactory<TUser>
    where TUser : class, IUser
{
    Task<ClaimsPrincipal> CreateAsync(
        string authenticationType,
        CookiePrincipalContext<TUser> context,
        CancellationToken cancellationToken = default);
}
