using EtherGizmos.Common.Abstractions;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class CarrierUnknownStatusRouter : IDomainEventRouter<CarrierUnknownStatusEvent>
{
    public async IAsyncEnumerable<string> FilterScopeAsync(
        CarrierUnknownStatusEvent @event,
        IEnumerable<AudienceKey> audiences,
        IEnumerable<string> userIds,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!audiences.Any()) yield break;

        foreach (var userId in userIds)
        {
            yield return userId;
        }
    }
}
