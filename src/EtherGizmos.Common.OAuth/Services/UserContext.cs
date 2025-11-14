using EtherGizmos.Common.Abstractions;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Guid? UserId => MaybeGuid(_httpContextAccessor.HttpContext?.User?.GetClaim(Claims.Subject));

    public UserContext(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid? MaybeGuid(
        string? input)
        => !string.IsNullOrWhiteSpace(input)
            ? Guid.TryParse(input, out var output)
            ? output
            : null 
            : null;
}
