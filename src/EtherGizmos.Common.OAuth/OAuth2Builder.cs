using EtherGizmos.Common.Abstractions;
using Microsoft.Extensions.Hosting;

namespace EtherGizmos.Common;

internal class OAuth2Builder : IOAuth2Builder
{
    public IHostApplicationBuilder Builder { get; }

    public OAuth2Builder(
        IHostApplicationBuilder builder)
    {
        Builder = builder;
    }
}
