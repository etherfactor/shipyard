namespace EtherGizmos.Shipyard.Database.Services;

public interface IUnitOfWork : IDisposable
{
    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    IRepository<TEntity> Repository<TEntity>()
        where TEntity : class;
}
