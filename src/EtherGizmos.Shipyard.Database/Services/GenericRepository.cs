using EtherGizmos.Shipyard.Utilities.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Database.Services;

internal class GenericRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> _entities;

    public GenericRepository(
        DbSet<TEntity> entities)
    {
        _entities = entities;
    }

    public IQueryable<TEntity> Data => _entities;

    public void Attach(TEntity entity)
    {
        _entities.Attach(entity);
    }

    public void Create(TEntity entity)
    {
        _entities.Add(entity);
    }

    public void Delete(TEntity entity)
    {
        _entities.Remove(entity);
    }

    public void Detach(TEntity entity)
    {
        _entities.Entry(entity).State = EntityState.Detached;
    }
}
