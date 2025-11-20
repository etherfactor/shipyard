using System.Security.Claims;

namespace EtherGizmos.Common.Abstractions;

public interface IOAuth2PrincipalFactory
{
    Task<ClaimsPrincipal> CreateAsync(
        string authenticationType,
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default);
}
