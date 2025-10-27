using Microsoft.Extensions.Hosting;

namespace EtherGizmos.Common.Abstractions;

public interface IOAuth2Builder
{
    public IHostApplicationBuilder Builder { get; }
}
