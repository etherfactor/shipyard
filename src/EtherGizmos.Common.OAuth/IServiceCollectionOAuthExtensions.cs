using EtherGizmos.Common.Abstractions;
using Microsoft.Extensions.Hosting;

namespace EtherGizmos.Common;

public static class IServiceCollectionOAuthExtensions
{
    public static IOAuth2Builder UseOAuth2(
        this IHostApplicationBuilder @this)
    {
        return new OAuth2Builder(@this);
    }
}
