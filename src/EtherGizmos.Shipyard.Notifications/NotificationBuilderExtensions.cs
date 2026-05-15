using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Events;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EtherGizmos.Shipyard;

public static class NotificationBuilderExtensions
{
    extension(INotificationBuilder @this)
    {
        public void AddShipyardExtractors()
        {
            @this.Services.TryAddSingleton<IDomainEventExtractor, PackageDeliveredExtractor>();
        }
    }
}
