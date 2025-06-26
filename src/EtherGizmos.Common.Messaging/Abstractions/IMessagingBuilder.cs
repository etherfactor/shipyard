using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Common.Messaging.Abstractions;

public interface IMessagingBuilder
{
    public IServiceCollection Services { get; }
}
