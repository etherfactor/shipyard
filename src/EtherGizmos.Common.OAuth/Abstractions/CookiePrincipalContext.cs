using Microsoft.AspNetCore.Http;
using System.Collections.Immutable;
using System.Security.Claims;

namespace EtherGizmos.Common.Abstractions;

public record CookiePrincipalContext<TUser>(
    HttpContext HttpContext,
    TUser User)
    : IClaimsContext
    where TUser : class, IUser
{
    public ImmutableArray<Claim> Claims { get; init; } = ImmutableArray<Claim>.Empty;

    public static CookiePrincipalContext<TUser> FromUser(
        HttpContext context,
        TUser user)
    {
        return new(context, user);
    }

    public CookiePrincipalContext<TUser> WithClaim(
        Claim claim)
        => this with { Claims = Claims.Add(claim) };

    public CookiePrincipalContext<TUser> WithClaims(
        IEnumerable<Claim> claims)
        => this with { Claims = Claims.AddRange(claims) };
}
