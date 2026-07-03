using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EtherGizmos.Shipyard;

public static class NotificationBuilderExtensions
{
    extension(INotificationBuilder @this)
    {
        public void AddShipyardExtractors()
        {
            @this.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IDomainEventExtractor), typeof(PackageDeliveredExtractor), ServiceLifetime.Singleton));
            @this.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IDomainEventExtractor), typeof(PackageOutForDeliveryExtractor), ServiceLifetime.Singleton));
        }
    }
}
