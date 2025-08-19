using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Abstractions;

public interface IUnitOfWork : IDisposable
{
    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    IRepository<TEntity> Repository<TEntity>()
        where TEntity : class, IEntity;
}
