using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EtherGizmos.Common.Abstractions;

public record CookiePrincipalContext<TUser>(
    HttpContext HttpContext,
    TUser User)
    : IClaimsContext
    where TUser : class, IUser
{
    public ClaimsIdentity Identity { get; init; } = new();

    public static CookiePrincipalContext<TUser> FromUser(
        HttpContext context,
        TUser user)
    {
        return new(context, user);
    }
}
