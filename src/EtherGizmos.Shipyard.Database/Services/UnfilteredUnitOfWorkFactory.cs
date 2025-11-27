using EtherGizmos.Shipyard.Abstractions;

namespace EtherGizmos.Shipyard.Services;

internal class UnfilteredUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IUnitOfWorkFactory _inner;

    public UnfilteredUnitOfWorkFactory(
        IUnitOfWorkFactory inner)
    {
        _inner = inner;
    }

    public IUnitOfWork Create()
        => new UnfilteredUnitOfWork(_inner.Create());

    public IUnitOfWork Create(bool useRequestScope)
        => new UnfilteredUnitOfWork(_inner.Create(useRequestScope));

    public IUnitOfWork Create(IServiceProvider provider)
        => new UnfilteredUnitOfWork(_inner.Create(provider));
}
