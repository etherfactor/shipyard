#pragma warning disable IDE0130
using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Events;

public class PackageDeliveredRouter : IDomainEventRouter<PackageDeliveredEvent>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public PackageDeliveredRouter(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public IAsyncEnumerable<string> FilterScopeAsync(
        PackageDeliveredEvent @event,
        IEnumerable<AudienceKey> audiences,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
