using EtherGizmos.Shipyard.Abstractions;

namespace EtherGizmos.Shipyard.Services;

internal class UnfilteredUnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWork _inner;

    public UnfilteredUnitOfWork(
        IUnitOfWork inner)
    {
        _inner = inner;
    }

    public void Dispose()
        => _inner.Dispose();

    public int SaveChanges()
        => _inner.SaveChanges();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _inner.SaveChangesAsync(cancellationToken);

    IRepository<TEntity> IUnitOfWork.Repository<TEntity>()
        => new UnfilteredRepository<TEntity>(_inner.Repository<TEntity>());
}
