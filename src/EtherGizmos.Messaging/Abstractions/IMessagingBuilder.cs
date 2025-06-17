using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Messaging.Abstractions;

public interface IMessagingBuilder
{
    public IServiceCollection Services { get; }
}
