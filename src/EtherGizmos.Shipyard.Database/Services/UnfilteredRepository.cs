using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Services;

internal class UnfilteredRepository<TEntity>
    : IRepository<TEntity>
    where TEntity : class, IEntity
{
    private readonly IRepository<TEntity> _inner;

    public UnfilteredRepository(
        IRepository<TEntity> inner)
    {
        _inner = inner;
    }

    public IQueryable<TEntity> Data
        => _inner.Data.IgnoreQueryFilters();

    public void Attach(TEntity entity)
        => _inner.Attach(entity);

    public void Create(TEntity entity)
        => _inner.Create(entity);

    public void Delete(TEntity entity)
        => _inner.Delete(entity);

    public void Detach(TEntity entity)
        => _inner.Detach(entity);

    public Task<TEntity> ReloadAsync(TEntity entity, CancellationToken cancellationToken = default)
        => _inner.ReloadAsync(entity, cancellationToken);
}
