using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Services;

namespace EtherGizmos.Shipyard.Extensions;

public static class IUnitOfWorkFactoryExtensions
{
    public static IUnitOfWorkFactory AsUnfiltered(
        this IUnitOfWorkFactory @this)
    {
        return new UnfilteredUnitOfWorkFactory(@this);
    }
}
