using EtherGizmos.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Messaging.Services;

internal class MessagingBuilder : IMessagingBuilder
{
    public IServiceCollection Services { get; }

    public MessagingBuilder(
        IServiceCollection services)
    {
        Services = services;
    }
}
